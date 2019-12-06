using NUnit.Framework;

namespace Analyzer.Tests
{
    public class CsharpAnalyzerTests
    {
        [Test]
        public void X_Can_Be_1_4_5_6_Common_Case()
        {
            var code = @"
            public class Main {

                public static void Method(bool[] conditions) {

                    int x;

                    x = 1;

                    if (conditions[0]) {

                        x = 2;

                        if (conditions[1]) {

                            x = 3;
                        }

                        x = 4;

                        if (conditions[2]) {

                            x = 5;
                        }
                    }

                    if (conditions[3]) {

                        x = 6;
                    }

                    Console.WriteLine(x);
                }
            }";

            var result = CsharpAnalyzer.AnalyzeInternal(code);
            
            CollectionAssert.AreEqual(new [] { "1", "4", "5", "6" }, result);
        }
        
        [Test]
        public void X_Can_Be_1_4_Last_Value_In_Inner_Scope()
        {
            var code = @"
            public class Main {

                public static void Method(bool[] conditions) {

                    int x;

                    x = 1;

                    if (conditions[0]) {

                        x = 2;

                        x = 3;

                        x = 4;
                    }

                    Console.WriteLine(x);
                }
            }";

            var result = CsharpAnalyzer.AnalyzeInternal(code);
            
            CollectionAssert.AreEqual(new [] { "1", "4" }, result);
        }
        
        [Test]
        public void X_Can_Be_4_Ignore_All_Behind()
        {
            var code = @"
            public class Main {

                public static void Method(bool[] conditions) {

                    int x;

                    if (conditions[0]) {

                        x = 1;

                        x = 2;

                        x = 3;
                    }

                    x = 4

                    Console.WriteLine(x);
                }
            }";

            var result = CsharpAnalyzer.AnalyzeInternal(code);
            
            CollectionAssert.AreEqual(new [] { "4" }, result);
        }
        
        [Test]
        public void X_Can_Be_1_2_3_All_Conditions()
        {
            var code = @"
            public class Main {

                public static void Method(bool[] conditions) {

                    int x;

                    if (conditions[0]) {

                        x = 1;
                    }

                    if (conditions[1]) {

                        x = 2;
                    }

                    if (conditions[3]) {

                        x = 3;
                    }

                    Console.WriteLine(x);
                }
            }";

            var result = CsharpAnalyzer.AnalyzeInternal(code);
            
            CollectionAssert.AreEqual(new [] { "1", "2", "3" }, result);
        }
        
        [Test]
        public void X_Can_Be_1_2_3_When_Any_Inner_Scope_Is_Possible()
        {
            var code = @"
            public class Main {

                public static void Method(bool[] conditions) {

                    int x;

                    if (conditions[0]) {

                        x = 1;

                        if (conditions[1]) {

                            x = 2;

                            if (conditions[3]) {

                                x = 3;
                            }
                        }
                    }

                    Console.WriteLine(x);
                }
            }";

            var result = CsharpAnalyzer.AnalyzeInternal(code);
            
            CollectionAssert.AreEqual(new [] { "1", "2", "3" }, result);
        }
    }
}