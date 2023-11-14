﻿using NLog;
using System.Linq;
using System.Security.Cryptography.Xml;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();
logger.Info("Program started");

var selection = "";

try
{
    do
    {
        Console.WriteLine("Enter your selection:");
        Console.WriteLine("1) Display all blogs");
        Console.WriteLine("2) Add Blog");
        Console.WriteLine("3) Create Post");
        Console.WriteLine("4) Display Posts");
        Console.WriteLine("Enter q to Quit.");

        selection = Console.ReadLine();

        int selectNum = 0;
        bool isNum = int.TryParse(selection, out selectNum);

        if(!isNum)
        {
            logger.Info($"Your enter: {selection}");
            continue;
        }

        var db = new BloggingContext();

        switch (selectNum)
        {
            case 1: //Display all blogs
                // Display all Blogs from the database
                var query = db.Blogs.OrderBy(b => b.Name);

                Console.WriteLine("All blogs in the database:");
                int n = 1;
                foreach (var item in query)
                {
                    //Console.WriteLine($"{n}) {item.Name}");
                    Console.WriteLine($"{item.Name}");
                    n++;
                }
            break;

            case 2: //Add blog
                // Create and save a new Blog
                Console.Write("Enter a name for a new Blog: ");
                var name = Console.ReadLine();

                var blog = new Blog { Name = name };

                db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);
            break;

            case 3: //Create Post

                Console.WriteLine("Select the blog you would to post to:");

                // Display all Blogs from the database
                var queryL = db.Blogs.OrderBy(b => b.Name);
                int i = 1;
                foreach (var item in queryL)
                {
                    Console.WriteLine($"{i}) {item.Name}");
                    i++;
                }

                var selectBlog = Console.ReadLine();

                long selectBlogNum = 0;
                bool changeN = long.TryParse(selectBlog, out selectBlogNum);

                if(!changeN)
                {
                    logger.Error("Invalid Blog ID");
                    break;
                }

                if(selectBlogNum > i || selectBlogNum < 1)
                {
                    logger.Error("There are no Blogs saved with that ID");
                    break;
                }

                int blogN = 1;
                var blog1 = new Blog {};

                foreach (var bl in queryL)
                {
                    if( blogN == selectBlogNum)
                    {
                        blog1 = bl;
                    }
                    blogN ++;
                }

                Console.WriteLine("Enter the Post title");
                var postTitle = Console.ReadLine();

                if(postTitle == "")
                {
                    logger.Error("Post title cannot be null");
                    break;                    
                }

                Console.WriteLine("Enter the Post content");
                var postContent = Console.ReadLine();

                var post = new Post { Title = postTitle, Content = postContent, Blog = blog1};

                //db.AddPost(post);
                logger.Info($"Post added - \"{postTitle}\"");

                
            break;

            case 4: //Display Posts

                Console.WriteLine("Select the blog's posts to display:");

                Console.WriteLine("0) Posts from all blogs");

                // Display all Blogs from the database
                var queryLi = db.Blogs.OrderBy(b => b.Name);
                int k = 1;
                foreach (var item in queryLi)
                {
                    Console.WriteLine($"{k}) Posts from {item.Name}");
                    k++;
                }

                var sb = Console.ReadLine();

                long sbn = 0;
                bool cn = long.TryParse(sb, out sbn);

                if(!cn)
                {
                    logger.Error("Invalid Blog ID");
                    break;
                }

                if(sbn > k || sbn < 0)
                {
                    logger.Error("There are no Blogs saved with that ID");
                    break;
                }

                // post returned
                if(sbn == 0)
                {
                    Console.WriteLine($"{db.Posts.Count()} post(s) returned");

                    foreach(var item in db.Posts)
                    {
                        Console.WriteLine($"Blog: {item.Blog}");
                        Console.WriteLine($"Title: {item.Title}");
                        Console.WriteLine($"Content: {item.Content} \n");
                    }
                }
                else
                {
                    int numID = 1;
                    foreach(var item in db.Posts)
                    {
                        if(numID == sbn)
                        {
                            Console.WriteLine($"Blog: {item.Blog}");
                            Console.WriteLine($"Title: {item.Title}");
                            Console.WriteLine($"Content: {item.Content} \n");
                        }

                        numID++;
                    }
                }
            break;
        }

        Console.WriteLine("\n");

    } while(selection.ToUpper() != "Q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");