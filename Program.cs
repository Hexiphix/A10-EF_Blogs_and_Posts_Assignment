using Castle.Core.Internal;
using EFTutorial.Models;
using System.Reflection.Metadata;

namespace EFBlogPost
{
    public class Program
    {
        static void Main(string[] args)
        {
            var choice = "a";

            // Enter q to quit
            while (choice != "q")
            {
                Console.WriteLine("\nEnter your selection:");
                Console.WriteLine("1) Display all blogs");
                Console.WriteLine("2) Add Blog");
                Console.WriteLine("3) Display Posts");
                Console.WriteLine("4) Add Post");
                Console.WriteLine("Enter q to quit");

                choice = Console.ReadLine();

                switch (choice)
                {
                    // 1) Display all blogs
                    case "1":
                        using (var db = new BlogContext())
                        {
                            Console.WriteLine($"\n{db.Blogs.Count()} Blog(s) found");
                            foreach (var b in db.Blogs)
                            {
                                Console.WriteLine($"{b.BlogId}) {b.Name}");
                            }
                        }
                        break;

                    // 2) Add Blog
                    case "2":
                        Console.Write("Enter name for a new Blog: ");
                        var blogName = Console.ReadLine();

                        // Ensures no blogs are null or without a title
                        if(string.IsNullOrWhiteSpace(blogName))
                        {
                            Console.WriteLine("\nBlog must have a name");
                        }
                        else 
                        { 
                            // Create new Blog
                            var newBlog = new Blog();
                            newBlog.Name = blogName;

                            // Save blog object to database
                            using (var db = new BlogContext())
                            {
                                db.Add(newBlog);
                                db.SaveChanges();
                            }
                        }
                        break;

                    // 3) Display Posts
                    case "3":
                        Console.WriteLine("\nSelect the blog's id you would like to display posts for:");
                        Console.WriteLine("0) Posts from all blogs");
                        using (var db = new BlogContext())
                        {
                            foreach (var b in db.Blogs)
                            {
                                Console.WriteLine($"{b.BlogId}) Posts from {b.Name}");
                            }

                            // Try catches if something other than a number is input for the id
                            try { 
                                var blogId = Convert.ToInt32(Console.ReadLine());

                                // Allows for all posts to be output
                                if(blogId == 0)
                                {
                                    /* Design Note: both foreachs within this particualar if statement ended up being extreamly picky
                                       
                                                    you can't use [ blog.Posts ] in a [ foreach (var blog in db.Blogs) ] anywhere at all

                                                    and you also can't use [ var blog = db.Blogs.Where(x => x.BlogId == post.BlogId).FirstOrDefault(); ]
                                                    from within a [ foreach (var post in db.Posts) ]

                                                    despite both these restrictions [post.Blog.Name] can be accessed in [ foreach (var post in db.Posts) ]
                                                    without any issues to speak of, likely having to do with the fact that no further list is accessed this, but its still picky
                                    */

                                    // Counts the full total number of posts
                                    int postCount = 0;
                                    foreach (var p in db.Posts)
                                    {
                                        postCount ++;
                                    }
                                    Console.WriteLine($"\n{postCount} post(s) found");

                                    // Outputs all posts with all relevent information
                                    foreach (var post in db.Posts)
                                    {
                                        Console.WriteLine($"Blog: {post.Blog.Name} (Blog Id:{post.BlogId})");
                                        Console.WriteLine($"Title: {post.Title}");
                                        Console.WriteLine($"Content: {post.Content}\n");
                                    }
                                } 
                                // Allows for just one blog's posts to be output
                                else { 
                                    var blog = db.Blogs.Where(x => x.BlogId == blogId).FirstOrDefault();
                                    // var blogsList = blog.ToList(); // convert to List from IQueryable

                                    // If statement catches if a number was input for an id that doesn't exist
                                    if(blog == null)
                                    {
                                        Console.WriteLine("\nThere are no Blogs saved with that Id");
                                    }
                                    else {
                                        // Outputs each all posts for the specified blog with all relevent information
                                        Console.WriteLine($"\n{blog.Posts.Count()} post(s) found for Blog {blog.Name} (Blog Id:{blog.BlogId})");
                                        foreach (var post in blog.Posts)
                                        {
                                            Console.WriteLine($"Title: {post.Title}");
                                            Console.WriteLine($"Content: {post.Content}\n");
                                        }
                                    }
                                }
                            } // Catches if something other than a number is input for the id and outputs the coresponding message
                            catch (FormatException) {
                                Console.WriteLine("\nInvalid Blog Id Entered");
                            }
                        }
                        break;

                    // 4) Add Post
                    case "4":
                        Console.WriteLine("\nSelect the blog's id you would like to add a post to:");
                        using (var db = new BlogContext())
                        {
                            foreach (var b in db.Blogs)
                            {
                                Console.WriteLine($"{b.BlogId}) {b.Name}");
                            }

                            // Try catches if something other than a number is input for the id
                            try
                            { 
                                var blogId = Convert.ToInt32(Console.ReadLine());

                                // If statement catches if a number was input for an id that doesn't exist
                                if (db.Blogs.Where(x => x.BlogId == blogId).FirstOrDefault() == null)
                                {
                                    Console.WriteLine("\nThere are no Blogs saved with that Id");
                                }
                                else { 
                                    Console.WriteLine("Enter the Post title");
                                    var postTitle = Console.ReadLine();

                                    // Ensures no posts are null or without a title
                                    if (string.IsNullOrWhiteSpace(postTitle))
                                    {
                                        Console.WriteLine("Post must have a title");
                                    }
                                    else {
                                        Console.WriteLine("Enter the Post content");
                                        var postContent = Console.ReadLine();

                                        // Creates new Post
                                        var newPost = new Post();
                                        newPost.Title = postTitle;
                                        newPost.Content = postContent;
                                        newPost.BlogId = blogId;

                                        // Saves post object to database
                                        db.Posts.Add(newPost);
                                        db.SaveChanges();
                                    }
                                }
                            } // Catches if something other than a number is input for the id and outputs the coresponding message
                            catch (FormatException)
                            {
                                Console.WriteLine("\nInvalid Blog Id Entered");
                            }
                        }
                        break;

                    // Something else is chosen
                    default:
                        break;
                }
            }
        }
    }
}