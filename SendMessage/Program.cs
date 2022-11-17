using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SendMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            var url = ConfigurationManager.AppSettings["url"];
            var token = ConfigurationManager.AppSettings["token"];

            var userListUrl = $"{url}/users.list?pretty=1";
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            
            while (true)
            {
                HttpResponseMessage response = client.GetAsync(userListUrl).Result;
                var users = response.Content.ReadAsStringAsync().Result;
                dynamic memberModel = JsonConvert.DeserializeObject<dynamic>(users);
                List<string> userList = new List<string>();
                List<string> userName = new List<string>();
                var cnt = 1;
                foreach (var member in memberModel.members)
                {
                    userList.Add((string)member.id);
                    userName.Add((string)member.name);
                    Console.WriteLine(cnt + ": " + member.name);
                    cnt++;
                }

                Console.WriteLine(cnt + ": " + "Exit");
                Console.WriteLine("For send message try to use this formet--- @username message_code ");
                var messages= new List<string>()
                    {
                        "Ki khobor? Update?",
                        "Hi!!",
                        "Hello",
                        "How are you?",
                        "What's up"
                    };
                string input = Console.ReadLine();
                if (input == "4" || input=="Exit" || input=="exit" || input=="EXIT")
                {
                    Console.WriteLine("OK!Program stopped");
                    break;
                }
                string nameWithSpeach = input.Substring(1, input.IndexOf(" "));
                string name = nameWithSpeach.Replace(" ", "");
                var index = userName.IndexOf(name);

                var code = Convert.ToInt32(input.Substring(input.Length- 1));
                if (index==-1)
                {
                    Console.WriteLine("That user not found");
                    continue;
                }
                
                
                var channelListUrl = $"{url}/chat.postMessage";
                var newPost = new Post()
                {
                    channel = userList[index],
                    text = messages[code-1]
                };
                
                var newPostJson = JsonConvert.SerializeObject(newPost);
                var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");
                var result = client.PostAsync(channelListUrl, payload).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Message Send Sucessfully ");
            }
            Console.ReadKey();
        }
    }
}
