1.How long did you spend on the coding assignment? What would you add to your solution if you had more time? If you didn't spend much time on the coding assignment then use this as an opportunity to explain what you would add.
Answer: I spent around 30 hours on this assignment. I have implemented various features to the application but I would enhance the error handling and validation mechanisms to provide more informative and consistent error responses to the API clients.
This would include handling exceptions, returning appropriate HTTP status codes, and providing meaningful error messages.I would also spend more time designing and implementing additional test cases to cover various scenarios.

2.What was the most useful feature that was added to the latest version of your language of choice?
Please include a snippet of code that shows how you've used it.
Answer:RateLimiting feature was added to .net core 7. Ratelimiting middleware is configured to limit the requests according to the configuration.If limit exceeds error is thrown.
   builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedwindow", option =>
            {
                option.Window = TimeSpan.FromSeconds(10);
                option.PermitLimit = 1;
                option.QueueLimit = 1;
                option.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            }));

3.How would you track down a performance issue in production? Have you ever had to do this?
Answer: There can be various causes of performance issue on production environment. Firstly we must identify the scope. Maintaining the logs can be useful in this case. Monitoring the logs can help identify the cause of performance degradation.
Different profiling tools can be used to identify the issues.Some sql operations can also cause performance degradation.
I had once faced issue in production of a legacy application where an internal stored procedure was taking time due to cross join issue on table. It was found by using sql profilier and then resolved.

4. What was the latest technical book you have read or tech conference you have been to? What did you
learn?
Answer: I have not read any latest technical book, but I try to learn by watching youtube videos or by doing trainings on microsoft website. I am currently persuing training in Azure cloud from microsoft website.

5.What do you think about this technical assessment?
Answer: This assignment is very thoughtful and practical way to assess a candidate. The user stroy was a practical and real world example. Also the requirements were clear and upto the mark.

6.Please, describe yourself using JSON.
{
	"name": "Rushma Lopes",
	"nationality": "Indian",
	"livesIn": "Amstelveen",
	"family": {
		"husband": "An IT professional working with Deloitte.",
		"daughter": "5 years old will be starting school from September."
	},
	"loves": [
		"coding",
		"spending time with family",
		"cooking",
		"shopping",
		"making friends"
	],
	"strength": [
		"adaptable",
		"persistent"
	],
	"education": "Bachelor or Engineering",
	"profession": "Full Stack .Net Developer"
}
6/6.
