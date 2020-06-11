using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfScreenHelper;

namespace ARESCore.Starter
{
  internal class ScreenMapper: IScreenMapper
  {

    public ScreenMapper()
    {
      var allscreens = Screen.AllScreens;
      if(!allscreens.Any())
      {
        // okay, this really shouldn't happen
      }
      else if(allscreens.Count() == 1)
      {
        // everybody is the same
       var onescreen = allscreens.FirstOrDefault();
        TopScreen = onescreen;
        PrimaryScreen = onescreen;
        SecondaryScreen = onescreen;
      }
      else if(allscreens.Count() == 2)
      {
        // main on primary, console on secondary, the "topscreen" is secondary
        foreach ( var screen in allscreens)
        {
          if ( screen.Primary )
            PrimaryScreen = screen;
          else
          {
            TopScreen = screen;
            SecondaryScreen = screen;
          }
        }
        
      }
      else
      {
        // main on primary, console on secondary (the farthest left that isn't main), Topscreen is highest that isn't primary
        Screen topmost = allscreens.FirstOrDefault();
        Screen leftmost = allscreens.FirstOrDefault();
        foreach ( var screen in allscreens )
        {
          if ( screen.Primary )
            PrimaryScreen = screen;
          else
          {
            if ( screen.WorkingArea.Top < topmost.WorkingArea.Top || topmost.Primary )
              topmost = screen;
            if ( screen.WorkingArea.Left < leftmost.WorkingArea.Left || leftmost.Primary)
              leftmost = screen;
          }
        }
        SecondaryScreen = leftmost;
        if(!leftmost.Equals(topmost))
        {
          TopScreen = topmost;
        }
        else
        {
          // now we have a problem. Let's assign an unassigned screen for top
          TopScreen = allscreens.FirstOrDefault( s => !s.Equals( SecondaryScreen ) && !s.Primary );
        }
      }

    }

    public Screen PrimaryScreen { get; set; }

    public Screen SecondaryScreen { get; set; }

    public Screen TopScreen { get; set; }
  }
}
