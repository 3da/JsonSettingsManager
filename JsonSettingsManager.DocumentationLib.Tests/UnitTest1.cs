using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JsonSettingsManager.DocumentationLib.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private IList<MemberInfo> TestDocumentationForType(object obj, MemberInfo expected)
        {
            var manager = new DocumentationManager();

            var documentation = manager.Generate(obj);


            Assert.AreEqual(JsonConvert.SerializeObject(expected, Formatting.Indented), JsonConvert.SerializeObject(documentation[0], Formatting.Indented));

            return documentation;
        }

        [TestMethod]
        public void TestEnum()
        {
            TestDocumentationForType(typeof(MyEnum), new EnumInfo()
            {
                Name = nameof(MyEnum),
                Type = nameof(MyEnum),
                Items = new List<MemberInfo>()
                {
                    new MemberInfo()
                    {
                        Name = "Hello",
                        Type = "int",
                        Value = "0",
                    },
                    new MemberInfo()
                    {
                        Name = "World",
                        Type = "int",
                        Value = "1",
                    },
                    new MemberInfo()
                    {
                        Name = "ZXCVB",
                        Type = "int",
                        Value = "2",
                    }
                }
            });
        }

        [TestMethod]
        public void TestArray()
        {
            TestDocumentationForType(typeof(byte[]), new MemberInfo()
            {
                Name = "Byte[]",
                Type = "byte[]",
                Items = new List<MemberInfo>()
                {
                    new MemberInfo()
                    {
                        Name = "Item",
                        Type = "byte"
                    }
                }
            });
        }

        [TestMethod]
        public void TestSubSettings()
        {
            TestDocumentationForType(new SubSettings(), new MemberInfo()
            {
                Name = "SubSettings",
                Type = "SubSettings",
                Items = new List<MemberInfo>()
                {
                    new MemberInfo()
                    {
                        Name = "MyEnum",
                        Type = "MyEnum",
                        Items = new List<MemberInfo>()
                        {
                            new MemberInfo()
                            {
                                Name = "Hello",
                                Type = "int",
                                Value = "0",
                            },
                            new MemberInfo()
                            {
                                Name = "World",
                                Type = "int",
                                Value = "1",
                            },
                            new MemberInfo()
                            {
                                Name = "ZXCVB",
                                Type = "int",
                                Value = "2",
                            }
                        }
                    },
                    new MemberInfo()
                    {
                        Name = "StrArray",
                        Type = "string[]",
                        Items = new List<MemberInfo>()
                        {
                             new MemberInfo()
                             {
                                 Name = "Item",
                                 Type = "string"
                             }
                        }
                    }
                }
            });
        }


        [TestMethod]
        public void TestMethod1()
        {
            var expected = new ClassInfo()
            {
                Name = "Settings",
                Type = "Settings",
                Items = new List<MemberInfo>()
                {
                    new ClassInfo()
                    {
                        Name = "SubSettings",
                        Type = "SubSettings",
                        Items = new List<MemberInfo>()
                        {
                            new MemberInfo()
                            {
                                Name = "MyEnum",
                                Type = "MyEnum",
                                Items = new List<MemberInfo>()
                                {
                                    new MemberInfo()
                                    {
                                        Name = "Hello",
                                        Type = "int",
                                        Value = "0",
                                    },
                                    new MemberInfo()
                                    {
                                        Name = "World",
                                        Type = "int",
                                        Value = "1",
                                    },
                                    new MemberInfo()
                                    {
                                        Name = "ZXCVB",
                                        Type = "int",
                                        Value = "2",
                                    }
                                }
                            },
                            new MemberInfo()
                            {
                                Name = "StrArray",
                                Type = "string[]",
                                Items = new List<MemberInfo>()
                                {
                                    new MemberInfo()
                                    {
                                        Name = "Item",
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    },
                    new MemberInfo()
                    {
                        Name = "Number",
                        Type = "decimal",
                    },
                    new MemberInfo()
                    {
                        Name = "Str",
                        Type = "string"
                    },
                    new MemberInfo()
                    {
                        Name = "MyEnums",
                        Type = "MyEnum[]",
                        Items = new List<MemberInfo>()
                        {
                            new EnumInfo()
                            {
                                Name = "Item",
                                Type = "MyEnum",
                                Items = new List<MemberInfo>()
                                {
                                    new MemberInfo()
                                    {
                                        Name = "Hello",
                                        Type = "int",
                                        Value = "0",
                                    },
                                    new MemberInfo()
                                    {
                                        Name = "World",
                                        Type = "int",
                                        Value = "1",
                                    },
                                    new MemberInfo()
                                    {
                                        Name = "ZXCVB",
                                        Type = "int",
                                        Value = "2",
                                    }
                                }
                            }
                        }
                    }
                }
            };

            TestDocumentationForType(new Settings(), expected);
        }
    }
}
