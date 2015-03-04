using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace KnockoutJsCommentBindingClassifier
{
    internal static class KnockoutJsCommentBindingClassifierClassificationDefinition
    {
        /// <summary>
        /// Defines the "KnockoutJsCommentBindingClassifier" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("KnockoutJsCommentBindingClassifier")]
        internal static ClassificationTypeDefinition KnockoutJsCommentBindingClassifierType = null;
    }
}
