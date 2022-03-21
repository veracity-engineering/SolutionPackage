namespace DNVGL.Veracity.Services.Api.This.Abstractions.Models
{
    public class CreateUserOptions
    {
	    public string FirstName { get; set; }

	    public string LastName { get; set; }

	    public string Email { get; set; }

	    /// <summary>
	    /// Specify additional registration options, this is not mandatory 
	    /// </summary>
	    public RegistrationOptions Options { get; set; }
    }

    public class RegistrationOptions
    {
        /// <summary>
        /// Set this to false to take responsibility of sending the registration email to the user.
        /// </summary>
        public bool? SendMail { get; set; }

        /// <summary>
        /// Make the service create a default subscription for the newly created user
        /// </summary>
        public bool? CreateSubscription { get; set; }

        /// <summary>
        /// The service id to create subscription for
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Specify the accessLevel/role the user should have with the new subscription. Optional
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Specify the location to send the newly created user to after the registration process is completed
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The email address of the user or service that creates the new user account
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// The Name of the user or service that creates the new user account
        /// </summary>
        public string ContactName { get; set; }
    }
}
