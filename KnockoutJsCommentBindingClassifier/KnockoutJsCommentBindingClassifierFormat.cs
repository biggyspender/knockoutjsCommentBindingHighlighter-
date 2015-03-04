using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace KnockoutJsCommentBindingClassifier
{
    #region Format definition
    /// <summary>
    /// Defines an editor format for the KnockoutJsCommentBindingClassifier type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "KnockoutJsCommentBindingClassifier")]
    [Name("KnockoutJsCommentBindingClassifier")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.High)] //set the priority to be after the default classifiers
    internal sealed class KnockoutJsCommentBindingClassifierFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "KnockoutJsCommentBindingClassifier" classification type
        /// </summary>
        public KnockoutJsCommentBindingClassifierFormat()
        {
            this.DisplayName = "KnockoutJs Comment Bindings"; //human readable version of the name
            this.ForegroundColor = Colors.LawnGreen;
            this.ForegroundOpacity = 1d;
            //this.BackgroundColor = Color.FromRgb(127, 127, 127);
            //this.BackgroundOpacity = 0.3d;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
            //this.IsBold = true;
            //this.IsItalic = true;
        }
    }
    #endregion //Format definition
}
