﻿namespace AntlrUnitTests.ST4
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringTemplate;
    using Antlr.Runtime.JavaExtensions;

    [TestClass]
    public class TestCompiler : StringTemplateTestBase
    {
        [TestInitialize]
        public void setUp()
        {
            Compiler.subtemplateCount = 0;
        }

        [TestMethod]
        public void TestAttr()
        {
            string template = "hi <name>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, " +
                "write 0 2, " +
                "load_attr 1, " +
                "write 3 8";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestInclude()
        {
            string template = "hi <foo()>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, write 0 2, new 1, write 3 9";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , /foo]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestProp()
        {
            string template = "hi <a.b>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, write 0 2, load_attr 1, load_prop 2, write 3 7";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , a, b]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestProp2()
        {
            string template = "<u.id>: <u.name>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_attr 0, load_prop 1, write 0 5, load_str 2, write 6 7, " +
                "load_attr 0, load_prop 3, write 8 15";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[u, id, : , name]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestMap()
        {
            string template = "hi <name:bold>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, write 0 2, load_attr 1, load_str 2, map, write 3 13";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /bold]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestRepeatedMap()
        {
            string template = "hi <name:bold:italics>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, " +
                "write 0 2, " +
                "load_attr 1, " +
                "load_str 2, " +
                "map, " +
                "load_str 3, " +
                "map, " +
                "write 3 21";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /bold, /italics]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestRotMap()
        {
            string template = "hi <name:bold,italics>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, write 0 2, load_attr 1, load_str 2, load_str 3, rot_map 2, write 3 21";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /bold, /italics]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestAnonMap()
        {
            string template = "hi <name:{n | <n>}>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, write 0 2, load_attr 1, load_str 2, map, write 3 18";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /_sub1]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestIf()
        {
            string template = "go: <if(name)>hi, foo<endif>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, write 0 3, load_attr 1, brf 22, load_str 2, write 14 20";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestIfElse()
        {
            string template = "go: <if(name)>hi, foo<else>bye<endif>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, " +
                "write 0 3, " +
                "load_attr 1, " +
                "brf 25, " +
                "load_str 2, " +
                "write 14 20, " +
                "br 33, " +
                "load_str 3, " +
                "write 27 29";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, bye]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestElseIf()
        {
            string template = "go: <if(name)>hi, foo<elseif(user)>a user<endif>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, " +
                "write 0 3, " +
                "load_attr 1, " +
                "brf 25, " +
                "load_str 2, " +
                "write 14 20, " +
                "br 39, " +
                "load_attr 3, " +
                "brf 39, " +
                "load_str 4, " +
                "write 35 40";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, user, a user]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestElseIfElse()
        {
            string template = "go: <if(name)>hi, foo<elseif(user)>a user<else>bye<endif>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, " +
                "write 0 3, " +
                "load_attr 1, " +
                "brf 25, " +
                "load_str 2, " +
                "write 14 20, " +
                "br 50, " +
                "load_attr 3, " +
                "brf 42, " +
                "load_str 4, " +
                "write 35 40, " +
                "br 50, " +
                "load_str 5, " +
                "write 47 49";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, user, a user, bye]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestOption()
        {
            string template = "hi <name; separator=\"x\">";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, write 0 2, load_attr 1, options, load_str 2, store_option 3, write_opt 3 23";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, x]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestOptionAsTemplate()
        {
            string template = "hi <name; separator={, }>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, write 0 2, load_attr 1, options, new 2, store_option 3, write_opt 3 24";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /_sub1]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestOptions()
        {
            string template = "hi <name; anchor, wrap=foo(), separator=\", \">";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected =
                "load_str 0, " +
                "write 0 2, " +
                "load_attr 1, " +
                "options, " +
                "load_str 2, " +
                "store_option 0, " +
                "new 3, " +
                "store_option 4, " +
                "load_str 4, " +
                "store_option 3, " +
                "write_opt 3 44";
            string stringsExpected = // the ", , ," is the ", " separator string
                "[hi , name, true, /foo, , ]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
        }

        [TestMethod]
        public void TestEmptyList()
        {
            string template = "<[]>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected = "list, write 0 3";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestList()
        {
            string template = "<[a,b]>";
            CompiledTemplate code = new Compiler().Compile(template);
            string asmExpected = "list, load_attr 0, add, load_attr 1, add, write 0 6";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[a, b]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestEmbeddedRegion()
        {
            string template = "<@r>foo<@end>";
            // compile as if in root dir and in template 'a'
            CompiledTemplate code = new Compiler("/", "a").Compile(template);
            string asmExpected =
                "new 0, write 0 12";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[/region__a__r]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestRegion()
        {
            string template = "x:<@r()>";
            // compile as if in root dir and in template 'a'
            CompiledTemplate code = new Compiler("/", "a").Compile(template);
            string asmExpected =
                "load_str 0, write 0 1, new 1, write 2 7";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[x:, /region__a__r]";
            string stringsResult = code.strings.ToElementString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }
    }
}
