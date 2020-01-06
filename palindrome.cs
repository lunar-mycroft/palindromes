using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;

namespace palindrome {
    class Palindrome {

        static void error(string msg) {
            Console.WriteLine(msg);
            Console.ReadKey();
            System.Environment.Exit(-1);
        }

        static string url {
            get {
                return ConfigurationManager.AppSettings["url"];
            }
        }

        static IEnumerable<string> strings {
            get {
                string text = "";
                using (WebClient wc = new WebClient()) {
                    try {
                        text = wc.DownloadString(url);
                    } catch (WebException e) {
                        error("There was some sort of problem downloading the json.  Are you sure you gave the correct url?");
                    }
                }
                object[] stringList = { }; // Need this to make the compiler stop complaining.

                try {
                    JavaScriptSerializer translator = new JavaScriptSerializer();
                    var json = translator.Deserialize<dynamic>(text);
                    stringList = json["strings"];
                } catch(ArgumentException e) {
                    error("Invalid json.  Did you forget to link to a raw json file?");
                } catch(KeyNotFoundException e) {
                    error("This json does not appear to be formated like the example given");
                }

                foreach (Dictionary<string, dynamic> d in stringList) {
                    string s = "";
                    try {
                        s = d["str"];
                    } catch (KeyNotFoundException e) {
                        error("This json does not appear to be formated like the example given");
                    }
                    yield return s;
                }
            }
                    
                
                    
        }

        static string prepString(string str) {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
                if (!(char.IsPunctuation(c) || char.IsWhiteSpace(c)))
                    builder.Append(char.ToLower(c));

            return builder.ToString();
        }

        static IEnumerable<Tuple<char, char>> fromEnds(string s) {
            int i = 0; int j = s.Length - 1;
            while (i <= j) {
                yield return Tuple.Create(s[i++], s[j--]);
            }
        }

        static bool isPalindrome(string s) {
            return fromEnds(prepString(s)).All(tup => tup.Item1 == tup.Item2);
        }

        static void Main(string[] args) {

            foreach (string s in strings)
                Console.WriteLine(String.Format((isPalindrome(s) ? "\"{0}\" is a palindrome" : "\"{0}\" is not a palindrome"), s));

            Console.ReadKey();
        }
    }
}
