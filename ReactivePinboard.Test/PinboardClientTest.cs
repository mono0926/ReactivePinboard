using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

//using System.Windows.Threading;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Api.ReactivePinboard;
using Mono.Api.ReactivePinboard.Model;

namespace PinboardTest
{
    /// <summary>
    ///PinboardClientTest のテスト クラスです。すべての
    ///PinboardClientTest 単体テストをここに含めます
    ///</summary>
    [TestClass()]
    public class PinboardClientTest
    {
        private TestContext testContextInstance;

        private PinboardClient client;

        private void InitializeClient()
        {
            client = new PinboardClient("monoXi", "hogehoge");
        }

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
        //テストを作成するときに、次の追加属性を使用することができます:
        //
        //クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //各テストを実行する前にコードを実行するには、TestInitialize を使用
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //各テストを実行した後にコードを実行するには、TestCleanup を使用
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion 追加のテスト属性

        ///// <summary>
        /////Parse のテスト
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("ReactivePinboard.dll")]
        //public void ParseTest()
        //{
        //    PrivateObject param0 = new PrivateObject(typeof(PinboardClient), new object[] { "", "" });
        //    PinboardClient_Accessor target = new PinboardClient_Accessor(param0);
        //    string response = Properties.Resources.posts;
        //    //actual = target.ParsePosts( response).ToList();
        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("このテストメソッドの正確性を確認します。");
        //}

        /// <summary>
        ///Update のテスト
        ///</summary>
        [TestMethod()]
        public void UpdateTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.UpdateAsync(result =>
            {
                Console.WriteLine(TimeZoneInfo.ConvertTimeToUtc(result).ToString("yyyy-MM-ddThh:mm:ssZ"));//2010-12-11T19:48:02Z
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///Update のテスト
        ///</summary>
        [TestMethod()]
        public void UpdateRxTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.Update().Subscribe(result =>
                {
                    Console.WriteLine(TimeZoneInfo.ConvertTimeToUtc(result).ToString("yyyy-MM-ddThh:mm:ssZ"));//2010-12-11T19:48:02Z
                    completion.Set();
                });
            completion.WaitOne();
        }

        /// <summary>
        ///AddPostAsync のテスト
        ///</summary>
        [TestMethod()]
        public void AddPostAsyncTest()
        {
            InitializeClient();
            Post post = new Post
            {
                Url = "http://www.yahoo.co.jp",
                Title = "hoge",
                Description = "fuga",
                Tags = new string[] { "hoge", "fuga" },
                Time = DateTime.Now,
                Replace = false,
                Shared = false,
                ToReade = true,
            };
            var completion = new ManualResetEvent(false);
            client.AddPostAsync(post, result =>
            {
                Console.WriteLine(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///AddPostAsync のテスト
        ///</summary>
        [TestMethod()]
        public void AddPostTest()
        {
            InitializeClient();
            Post post = new Post
            {
                Url = "http://www.yahoo.co.jp/aaa",
                Title = "hoge2",
                Description = "fuga",
                Tags = new string[] { "hoge", "fuga", "aaa" },
                Time = DateTime.Now,
                Replace = true,
                Shared = true,
                ToReade = true,
            };
            var completion = new ManualResetEvent(false);
            client.AddPost(post).Subscribe(result =>
            {
                Console.WriteLine(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///DeletePostAsync のテスト
        ///</summary>
        [TestMethod()]
        public void DeletePostAsyncTest()
        {
            InitializeClient();
            var url = "http://parosky.net/m2viewer/?board=mnewsplus&threadnum=1328772941";
            var completion = new ManualResetEvent(false);
            client.DeletePostAsync(url, result =>
            {
                Console.WriteLine(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///GetAsync のテスト
        ///</summary>
        [TestMethod()]
        public void GetAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.GetAllBookmarks().Subscribe(result =>
            {
                var a = result.ToList();
                Console.WriteLine(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///GetPostAsync のテスト
        ///</summary>
        [TestMethod()]
        public void GetPostAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.GetPostAsync(date: DateTime.Now.AddDays(-1), includeMeta: true, callback: result =>
            {
                var a = result.ToList();
                Console.WriteLine(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///GetRecentAsync のテスト
        ///</summary>
        [TestMethod()]
        public void GetRecentAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.GetRecentAsync(count: 3, callback: result =>
            {
                var a = result.ToList();
                Console.WriteLine(a);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///GetDatesAsync のテスト
        ///</summary>
        [TestMethod()]
        public void GetDatesAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.GetDatesAsync(tags: new string[] { "mono_reader" }, callback: result =>
            {
                var a = result.ToList();
                Console.WriteLine(a);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///GetSuggestedAsync のテスト
        ///</summary>
        [TestMethod()]
        public void GetSuggestedAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.GetSuggestedAsync(@"http://blog.com/", callback: result =>
            {
                var a = result;
                Console.WriteLine(a);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///GetTagAsync のテスト
        ///</summary>
        [TestMethod()]
        public void GetTagAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.GetTagsAsync(callback: result =>
            {
                var a = result.ToList();
                Console.WriteLine(a);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///DeleteTagAsync のテスト
        ///</summary>
        [TestMethod()]
        public void DeleteTagAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.DeleteTagAsync("fuga", callback: result =>
            {
                Console.WriteLine(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///RenameTagAsync のテスト
        ///</summary>
        [TestMethod()]
        public void RenameTagAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.RenameTagAsync("lifehack", "lifehack2", callback: result =>
            {
                Console.WriteLine(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///SecretAsync のテスト
        ///</summary>
        [TestMethod()]
        public void SecretAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.GetUserSecretAsync(result =>
            {
                Console.WriteLine(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///Secret のテスト
        ///</summary>
        [TestMethod()]
        public void SecretTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);
            client.GetUserSecret().
            Subscribe(result =>
            {
                Assert.IsNotNull(result);
                client = new PinboardClient("aaa", "bakdajflajf");
                client.GetUserSecret()
                    .Subscribe(aaa =>
                {
                    Assert.IsNull(aaa);
                    completion.Set();
                }, () => Console.WriteLine("comp2"));
            }, () => Console.WriteLine("comp1"));

            completion.WaitOne();
        }

        [TestMethod()]
        public void SecretFailTest()
        {
            client = new PinboardClient("xafdsafdsfdsfasdfasd", "hogehoge");
            var completion = new ManualResetEvent(false);
            client.GetUserSecret().Subscribe(result =>
            {
                Assert.IsNull(result);
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///NetworkAsync のテスト
        ///</summary>
        [TestMethod()]
        public void NetworkAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);

            client.GetNetworkAsync(a =>
                {
                    var b = a;
                    completion.Set();
                });

            completion.WaitOne();
        }

        /// <summary>
        ///Network のテスト
        ///</summary>
        [TestMethod()]
        public void NetworkTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);

            client.GetNetwork().Subscribe(a =>
            {
                var b = a.ToArray();
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///Popular のテスト
        ///</summary>
        [TestMethod()]
        public void PopularAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);

            client.GetPopularAsync(a =>
            {
                var b = a.ToArray();
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///Network のテスト
        ///</summary>
        [TestMethod()]
        public void Popular()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);

            client.GetPopular().Subscribe(a =>
            {
                var b = a.ToArray();
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///Popular のテスト
        ///</summary>
        [TestMethod()]
        public void GetRecentAllAsyncTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);

            client.GetRecentAllAsync(a =>
            {
                var b = a.ToArray();
                completion.Set();
            });

            completion.WaitOne();
        }

        /// <summary>
        ///Network のテスト
        ///</summary>
        [TestMethod()]
        public void GetRecentAllTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);

            client.GetRecentAll().Subscribe(a =>
            {
                var b = a.ToArray();
                completion.Set();
            });

            completion.WaitOne();
        }

        //キャッシュ
        private IList<Post>[] allBookmarks = new IList<Post>[] { new Post[] { }.ToList() };

        private IObservable<IList<Post>> GetAllBookmarks()
        {
            return Observable.Defer<IList<Post>>(() =>
            {
                //キャッシュされていたらそれを利用
                if (client == null || allBookmarks[0].Count() != 0)
                {
                    return allBookmarks.ToObservable();
                }
                //初回
                return client.GetAllBookmarks().Do(posts =>
                {
                    allBookmarks[0] = posts.ToList();
                }).Select(x => (IList<Post>)x.ToList());
            });
        }

        [TestMethod()]
        [TestCategory("pinboard")]
        public void HogeTest()
        {
            InitializeClient();
            var completion = new ManualResetEvent(false);

            GetAllBookmarks().Subscribe(posts =>
                {
                    var p = posts;
                }, ex =>
                {
                    var a = ex;
                });

            completion.WaitOne();
        }
    }
}