using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Api.Hatena;
using Mono.Api.Hatena.Extensions;

namespace Mono.Api.Pinboard.Test
{
    /// <summary>
    /// UnitTest1 の概要の説明
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性

        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion 追加のテスト属性

        [TestMethod()]
        public void GeHotEntryTest()
        {
            var client = new HatenaClient();
            var completion = new ManualResetEvent(false);

            client.GetHotEntry().Subscribe(a =>
            {
                //var b = a.ToArray();
                completion.Set();
            });

            completion.WaitOne();
        }

        [TestMethod]
        public void GetRegImage()
        {
            var result = HatebuExtension.GetImageUrl(@"<blockquote cite=""http://nikkan-spa.jp/256437"" title=""「一日5分」で腹だけ凹む男のダイエット | 日刊SPA!""><cite><img src=""http://cdn-ak.favicon.st-hatena.com/?url=http%3A%2F%2Fnikkan-spa.jp%2F"" alt="""" />
			 <a hre");
            Assert.AreEqual("http://cdn-ak.favicon.st-hatena.com/?url=http%3A%2F%2Fnikkan-spa.jp%2F", result);
        }
    }
}