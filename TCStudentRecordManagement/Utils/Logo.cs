using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Utils
{
    public static class Logo
    {
        public static void printLogo()
        {
            // ASCII version of the TECHCareers logo.
            // Created using https://manytools.org/hacker-tools/convert-images-to-ascii-art/
            // Logo (C) TECHCareers / Manpower

            Console.Write("\n");
            ClWrite(ConsoleColor.White, "         /"); ClWrite(ConsoleColor.Red, "//\n");
            ClWrite(ConsoleColor.White, "   #"); ClWrite(ConsoleColor.Gray, "%%"); ClWrite(ConsoleColor.White, ".  /"); ClWrite(ConsoleColor.Red, "//  "); ClWrite(ConsoleColor.White, "&"); ClWrite(ConsoleColor.Gray, "%#"); ClWrite(ConsoleColor.White, "(\n");
            ClWrite(ConsoleColor.DarkGray, "  #"); ClWrite(ConsoleColor.Gray, "#     "); ClWrite(ConsoleColor.White, "/"); ClWrite(ConsoleColor.Red, "/"); ClWrite(ConsoleColor.White, "(    ,"); ClWrite(ConsoleColor.DarkGray, "#(\n");
            ClWrite(ConsoleColor.DarkGray, " (#              "); ClWrite(ConsoleColor.White, "#"); ClWrite(ConsoleColor.DarkGray, "("); ClWrite(ConsoleColor.White, "/\n"); 
            ClWrite(ConsoleColor.DarkGray, " (#    (((((("); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.DarkGray, "(("); ClWrite(ConsoleColor.White, "(/(( ,"); ClWrite(ConsoleColor.DarkGray, "(((((( ((   "); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.DarkGray, "("); ClWrite(ConsoleColor.White, "*"); ClWrite(ConsoleColor.Red, "//////\n");
            ClWrite(ConsoleColor.DarkGray, "  #"); ClWrite(ConsoleColor.Gray, "#     "); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.DarkGray, "("); ClWrite(ConsoleColor.White, ",  "); ClWrite(ConsoleColor.DarkGray, "/(((("); ClWrite(ConsoleColor.White, "(("); ClWrite(ConsoleColor.DarkGray, "("); ClWrite(ConsoleColor.White, "(      "); ClWrite(ConsoleColor.DarkGray, "(((((("); ClWrite(ConsoleColor.White, "."); ClWrite(ConsoleColor.Red, "//      "); ClWrite(ConsoleColor.White, ","); ClWrite(ConsoleColor.Red, "///"); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.Red, "/  ///"); ClWrite(ConsoleColor.White, "#."); ClWrite(ConsoleColor.Red, "////  "); ClWrite(ConsoleColor.White, "*"); ClWrite(ConsoleColor.Red, "////  ///"); ClWrite(ConsoleColor.White, "/"); ClWrite(ConsoleColor.Red, " ////\n");
            ClWrite(ConsoleColor.White, "   /"); ClWrite(ConsoleColor.Gray, "%%"); ClWrite(ConsoleColor.White, ".  ("); ClWrite(ConsoleColor.DarkGray, "("); ClWrite(ConsoleColor.White, ",  "); ClWrite(ConsoleColor.DarkGray, "(("); ClWrite(ConsoleColor.White, "#*   "); ClWrite(ConsoleColor.DarkGray, "((      ((   "); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.DarkGray, "/"); ClWrite(ConsoleColor.Red, "//"); ClWrite(ConsoleColor.White, "*     "); ClWrite(ConsoleColor.Red, "//  "); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.Red, "/  //  //"); ClWrite(ConsoleColor.White, "//// "); ClWrite(ConsoleColor.Red, "//"); ClWrite(ConsoleColor.White, "//// "); ClWrite(ConsoleColor.Red, "//   "); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.Red, "///\n"); 
            ClWrite(ConsoleColor.White, "         ("); ClWrite(ConsoleColor.DarkGray, "("); ClWrite(ConsoleColor.White, ",  "); ClWrite(ConsoleColor.DarkGray, "((((("); ClWrite(ConsoleColor.White, "(   "); ClWrite(ConsoleColor.DarkGray, "((((( ((   "); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.DarkGray, "("); ClWrite(ConsoleColor.White, "/ "); ClWrite(ConsoleColor.Red, "////"); ClWrite(ConsoleColor.White, "/  "); ClWrite(ConsoleColor.Red, "///"); ClWrite(ConsoleColor.White, "("); ClWrite(ConsoleColor.Red, "/  //   ////   ////  //   ///"); ClWrite(ConsoleColor.White, "/\n");
            Console.Write("\n");
        }


        // Wrapper function for Console.Write with colourised output
        public static void ClWrite(ConsoleColor clr, string text)
        {
            ConsoleColor currentClr = Console.ForegroundColor;
            Console.ForegroundColor = clr;
            Console.Write(text);
            Console.ForegroundColor = currentClr;
        }
    }

}
