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

namespace Day05
{
    class Program05
    {
        static void Main(string[] _)
        {
            // sanity checks
            foreach (var kvp in Data05.tests)
            {
                var result = ProcessCode(kvp.Key);
                Trace.Assert(kvp.Value.Equals(result));
            }

            Part1();
            Part2();
        }


        /*
         * --- Day 5: Binary Boarding ---
         * 
         * You board your plane only to discover a new problem: you dropped your boarding pass! You aren't sure which
         * seat is yours, and all of the flight attendants are busy with the flood of people that suddenly made it
         * through passport control.
         * 
         * You write a quick program to use your phone's camera to scan all of the nearby boarding passes (your puzzle
         * input); perhaps you can find your seat through process of elimination.
         * 
         * Instead of zones or groups, this airline uses binary space partitioning to seat people. A seat might be
         * specified like FBFBBFFRLR, where F means "front", B means "back", L means "left", and R means "right".
         * 
         * The first 7 characters will either be F or B; these specify exactly one of the 128 rows on the plane
         * (numbered 0 through 127). Each letter tells you which half of a region the given seat is in. Start with the
         * whole list of rows; the first letter indicates whether the seat is in the front (0 through 63) or the back
         * (64 through 127). The next letter indicates which half of that region the seat is in, and so on until you're
         * left with exactly one row.
         * 
         * For example, consider just the first seven characters of FBFBBFFRLR:
         * 
         * Start by considering the whole range, rows 0 through 127.
         * F means to take the lower half, keeping rows 0 through 63.
         * B means to take the upper half, keeping rows 32 through 63.
         * F means to take the lower half, keeping rows 32 through 47.
         * B means to take the upper half, keeping rows 40 through 47.
         * B keeps rows 44 through 47.
         * F keeps rows 44 through 45.
         * The final F keeps the lower of the two, row 44.
         * 
         * The last three characters will be either L or R; these specify exactly one of the 8 columns of seats on the
         * plane (numbered 0 through 7). The same process as above proceeds again, this time with only three steps. L
         * means to keep the lower half, while R means to keep the upper half.
         * 
         * For example, consider just the last 3 characters of FBFBBFFRLR:
         * 
         * Start by considering the whole range, columns 0 through 7.
         * R means to take the upper half, keeping columns 4 through 7.
         * L means to take the lower half, keeping columns 4 through 5.
         * The final R keeps the upper of the two, column 5.
         * So, decoding FBFBBFFRLR reveals that it is the seat at row 44, column 5.
         * 
         * Every seat also has a unique seat ID: multiply the row by 8, then add the column. In this example, the seat
         * has ID 44 * 8 + 5 = 357.
         * 
         * Here are some other boarding passes:
         * 
         * BFFFBBFRRR: row 70, column 7, seat ID 567.
         * FFFBBBFRRR: row 14, column 7, seat ID 119.
         * BBFFBBFRLL: row 102, column 4, seat ID 820.
         * 
         * As a sanity check, look through your list of boarding passes. What is the highest seat ID on a boarding pass?
         */
        static void Part1()
        {
            int maxval = 0;
            foreach (var pass in Data05.actual)
            {
                var result = ProcessCode(pass);
                maxval = result.Item3 > maxval ? result.Item3 : maxval;
            }
            Console.WriteLine(maxval);
            // Solution: 989
        }

        /**
         * --- Part Two ---
         * 
         * Ding! The "fasten seat belt" signs have turned on. Time to find your seat.
         * 
         * It's a completely full flight, so your seat should be the only missing boarding pass in your list. However,
         * there's a catch: some of the seats at the very front and back of the plane don't exist on this aircraft, so
         * they'll be missing from your list as well.
         * 
         * Your seat wasn't at the very front or back, though; the seats with IDs +1 and -1 from yours will be in your
         * list.
         * 
         * What is the ID of your seat?
         */
        static void Part2()
        {
            // True if the seat ID is available
            List<bool> results = Enumerable.Repeat(true, 990).ToList();
            foreach (var pass in Data05.actual)
            {
                var seatinfo = ProcessCode(pass);
                results[seatinfo.Item3] = false;
            }

            // 1,2,3,4,...,85,86,87,88,548
            var openSeatIndices = Enumerable.Range(0, results.Count)
                .Where(i => i > 0 && results[i])
                .ToList();

            // Filter out any seat IDs that neighbor open seat IDs
            for (int n = openSeatIndices.Count - 1; n >= 0; n--)
            {
                var idx = openSeatIndices[n];
                if (results[idx - 1] || results[idx + 1])
                {
                    openSeatIndices.Remove(idx);
                }
            }

            var output = string.Join(",", openSeatIndices.ToArray());
            Console.WriteLine(output);
            // Solution: 548
        }


        // Returns { row, column, seatID }
        static Tuple<int, int, int> ProcessCode(in string code)
        {
            int row = 0;
            int col = 0;

            for (int n = 0; n < 7; n++)
            {
                if (code[n] == 'B')
                {
                    row |= 1 << (6 - n);
                }
            }

            for (int n = 7; n < 10; n++)
            {
                if (code[n] == 'R')
                {
                    col |= 1 << (9 - n);
                }
            }

            int id = row * 8 + col;
            return new Tuple<int, int, int>(row, col, id);
        }
    }
}
