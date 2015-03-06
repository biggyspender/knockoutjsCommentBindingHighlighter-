using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace KnockoutJsCommentBindingClassifier
{

    #region Provider definition

    /// <summary>
    /// This class causes a classifier to be added to the set of classifiers. Since 
    /// the content type is set to "text", this classifier applies to all text files
    /// </summary>
    [Export(typeof (IClassifierProvider))]
    [ContentType("HTML")]
    internal class KnockoutJsCommentBindingClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import] internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer
                .Properties
                .GetOrCreateSingletonProperty<KnockoutJsCommentBindingClassifier>(
                    () => new KnockoutJsCommentBindingClassifier(ClassificationRegistry));
        }
    }

    #endregion //provider def

    #region Classifier

    /// <summary>
    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
    /// </summary>
    internal class KnockoutJsCommentBindingClassifier : IClassifier
    {
        private readonly IClassificationType classificationType;
        private readonly Stopwatch stopwatch;
        internal KnockoutJsCommentBindingClassifier(IClassificationTypeRegistryService registry)
        {
            classificationType = registry.GetClassificationType("KnockoutJsCommentBindingClassifier");
            stopwatch = Stopwatch.StartNew();
        }

        private readonly MemoryCache memoryCache = new MemoryCache(Guid.NewGuid().ToString("N"));

        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </summary>
        /// <param name="trackingSpan">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var textSnapshot = span.Snapshot;
            var text = textSnapshot.GetText();
            var knockoutNodePositions = GetKnockoutNodePositions(text);

            var classificationSpans = knockoutNodePositions.Where(
                np => np.Start >= span.Start.Position && np.Start < span.End.Position)
                .Select(
                    knp =>
                        new ClassificationSpan(new SnapshotSpan(textSnapshot, new Span(knp.Start, knp.Length)),
                            classificationType))
                .ToList();
            return classificationSpans;
        }

        private IEnumerable<Span> GetKnockoutNodePositions(string text)
        {
            return memoryCache.GetOrStore(text, () => KnockoutNodePositions(text), 10);
        }

      
        private static IEnumerable<Span> KnockoutNodePositions(string text)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            var knockoutNodes =
                doc.DocumentNode.Descendants()
                    .OfType<HtmlCommentNode>()
                    .Where(
                        n =>
                            Regex.IsMatch(n.Comment,
                                @"(?:^\<\!-- *ko +[A-Za-z0-9_]+ *\:[^\r\n]*?--\>$)|(?:^\<\!-- *\/ko *--\>$)",
                                RegexOptions.Singleline));
            var knockoutNodePositions = knockoutNodes.Select(n => new Span(n.StreamPosition, n.OuterHtml.Length));
            return knockoutNodePositions;
        }

#pragma warning disable 67
        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the classification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
    }

    #endregion //Classifier
}