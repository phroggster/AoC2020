/*
 * Solutions to the Advent Of Code 2020
 * Copyright © 2021 Leland Roach
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day06
{
    class Program06
    {
        static void Main(string[] _)
        {
            Part1();
            Part2();
        }

        /*
         * --- Day 6: Custom Customs ---
         * As your flight approaches the regional airport where you'll switch to a much larger plane, customs
         * declaration forms are distributed to the passengers.
         * 
         * The form asks a series of 26 yes-or-no questions marked a through z. All you need to do is identify the
         * questions for which anyone in your group answers "yes". Since your group is just you, this doesn't take very
         * long.
         * 
         * However, the person sitting next to you seems to be experiencing a language barrier and asks if you can
         * help. For each of the people in their group, you write down the questions for which they answer "yes", one
         * per line. For example:
         * 
         * abcx
         * abcy
         * abcz
         * 
         * In this group, there are 6 questions to which anyone answered "yes": a, b, c, x, y, and z. (Duplicate
         * answers to the same question don't count extra; each question counts at most once.)
         * 
         * Another group asks for your help, then another, and eventually you've collected answers from every group on
         * the plane (your puzzle input). Each group's answers are separated by a blank line, and within each group,
         * each person's answers are on a single line. For example:
         * 
         * abc
         * 
         * a
         * b
         * c
         * 
         * ab
         * ac
         * 
         * a
         * a
         * a
         * a
         * 
         * b
         * 
         * This list represents answers from five groups:
         * 
         * The first group contains one person who answered "yes" to 3 questions: a, b, and c.
         * The second group contains three people; combined, they answered "yes" to 3 questions: a, b, and c.
         * The third group contains two people; combined, they answered "yes" to 3 questions: a, b, and c.
         * The fourth group contains four people; combined, they answered "yes" to only 1 question, a.
         * The last group contains one person who answered "yes" to only 1 question, b.
         * In this example, the sum of these counts is 3 + 3 + 3 + 1 + 1 = 11.
         * 
         * For each group, count the number of questions to which anyone answered "yes". What is the sum of those counts?
         */
        static void Part1()
        {
            // sanity checks
            foreach (var kvp in Data06.tests)
            {
                var result = CountAny(kvp.Key);
                if (kvp.Value.Item1 != result)
                    throw new ApplicationException("Test data was not validated correctly, cannot continue");
            }

            int total = 0;
            foreach(var s in Data06.actual)
            {
                total += CountAny(s);
            }
            Console.WriteLine(total);
            // Solution: 6351
        }

        /*
         * --- Part Two ---
         * As you finish the last group's customs declaration, you notice that you misread one word in the instructions:
         * 
         * You don't need to identify the questions to which anyone answered "yes"; you need to identify the questions
         * to which everyone answered "yes"!
         * 
         * Using the same example as above:
         * 
         * abc
         * 
         * a
         * b
         * c
         * 
         * ab
         * ac
         * 
         * a
         * a
         * a
         * a
         * 
         * b
         * 
         * This list represents answers from five groups:
         * 
         * In the first group, everyone (all 1 person) answered "yes" to 3 questions: a, b, and c.
         * In the second group, there is no question to which everyone answered "yes".
         * In the third group, everyone answered yes to only 1 question, a. Since some people did not answer "yes" to b
         * or c, they don't count.
         * In the fourth group, everyone answered yes to only 1 question, a.
         * In the fifth group, everyone (all 1 person) answered "yes" to 1 question, b.
         * In this example, the sum of these counts is 3 + 0 + 1 + 1 + 1 = 6.
         * 
         * For each group, count the number of questions to which everyone answered "yes". What is the sum of those
         * counts?
        */
        static void Part2()
        {
            // sanity checks
            foreach (var kvp in Data06.tests)
            {
                var result = CountAll(kvp.Key);
                if (kvp.Value.Item2 != result)
                    throw new ApplicationException("Test data was not validated correctly, cannot continue");
            }

            int total = 0;
            foreach (var s in Data06.actual)
            {
                total += CountAll(s);
            }
            Console.WriteLine(total);
            // Solution: 3143
        }

        // Count the number of questions to which **anyone** answered yes
        static int CountAny(in string data)
        {
            List<char> listAny = new List<char>();

            foreach (var c in data)
            {
                if (!char.IsLetter(c) || listAny.Contains(c))
                    continue;
                else
                    listAny.Add(c);
            }

            return listAny.Count();
        }

        // Count the number of questions to which **everyone** answered yes
        static int CountAll(in string data)
        {
            // The number of lines in the data set represents the number of responses
            var nPeople = data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Count();
            Dictionary<char, int> charCounts = new Dictionary<char, int>();

            foreach(var c in data)
            {
                if (!char.IsLetter(c))
                    continue;
                else if (!charCounts.ContainsKey(c))
                    charCounts.Add(c, 1);
                else
                    charCounts[c]++;
            }

            return charCounts.Where(kvp => kvp.Value == nPeople).Count();
        }
    }
}
