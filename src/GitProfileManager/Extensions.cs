using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GitProfileManager
{
    public static class Extensions
    {
        public static string ToConfig(this string s) {
            return string.Join(Environment.NewLine, s.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries));
        }

        public static IEnumerable<string> ToConfig(this IEnumerable<string> ss) {
            return ss
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => !l.StartsWith("#"))
                .Select(l => l.Trim())
                .Select(l => {
                    if (l.StartsWith("git ")) {
                        // Console.WriteLine("removing git");
                        l = l.Replace("git", string.Empty).Trim();
                    }
                    return l;
                })
                .Select(l => {
                    if (l.StartsWith("config ")) {
                        //Console.WriteLine("removing config");
                        l = l.Replace("config ", string.Empty).Trim();
                    }
                    return l;
                })
                .Select(l => l.Trim());
        }
    }
}