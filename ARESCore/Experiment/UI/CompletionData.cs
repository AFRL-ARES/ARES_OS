using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ARESCore.Experiment.UI
{
  public class CompletionData : ICompletionData
  {
    public CompletionData( string text, string description )
    {
      this.Text = text;
      Description = description;
    }

    public System.Windows.Media.ImageSource Image
    {
      get { return null; }
    }

    public string Text { get; private set; }

    // Use this property if you want to show a fancy UIElement in the drop down list.
    public object Content
    {
      get { return this.Text; }
    }

    public object Description { get; private set; }

    public double Priority
    {
      get { return 0; }
    }

    public void Complete( TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs )
    {
      var wl = GetCurrentWordLength( textArea, completionSegment.Offset );
      ISegment replacementSegment = new TextSegment() { EndOffset = completionSegment.EndOffset, Length = completionSegment.Length + wl, StartOffset = completionSegment.Offset - wl };
      textArea.Document.Replace( replacementSegment, this.Text );
    }

    private int GetCurrentWordLength(TextArea textArea, int initialOffset)
    {
      var wordStart = TextUtilities.GetNextCaretPosition( textArea.Document, initialOffset, LogicalDirection.Backward, CaretPositioningMode.WordStart );
      if ( wordStart < 0 )
        return 0;
      return initialOffset - wordStart;
    }
  }
}
