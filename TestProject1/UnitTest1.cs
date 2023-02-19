using P.Http;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            PHttp http = new();

            var result = http.SetUrl("https://www.baidu.com/")
                .SetMethod(HttpMethod.Get)
                .Send();




            var s = new StreamReader(result.Content.ReadAsStream());
            var str = s.ReadToEnd();
            Console.WriteLine(str);




            http.SetUrl("https://www.baidu.com/")
               .SetMethod(HttpMethod.Get)
               .SetTimeOutSecond(5)
               .Send();


            http.SetUrl("https://www.baidu.com/")
               .SetMethod(HttpMethod.Get)
               .SetTimeOutSecond(6)
               .Send();





        }
    }
}