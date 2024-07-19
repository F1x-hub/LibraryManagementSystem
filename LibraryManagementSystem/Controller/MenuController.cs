using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controller
{
    internal class MenuController
    {
        public void LoadingAnimation()
        {
            int delay = 2000; 
            int animationInterval = 250; 
            int maxDots = 3; 

            DateTime endTime = DateTime.Now.AddMilliseconds(delay);
            int dotCount = 0;

            while (DateTime.Now < endTime)
            {
                Console.Write("."); 
                dotCount++;

                Thread.Sleep(animationInterval); 

                if (dotCount == maxDots)
                {
                    
                    for (int i = 0; i < maxDots; i++)
                    {
                        Console.Write("\b \b");
                    }
                    dotCount = 0; 
                }
            }

            
            for (int i = 0; i < dotCount; i++)
            {
                Console.Write("\b \b"); 
            }
        }
    }
}
