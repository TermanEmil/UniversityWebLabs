using System;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Presentation.Authorization
{
	public class ImgIsOwnerAuthorizationHandler :
	    AuthorizationHandler<OperationAuthorizationRequirement, ImgUpload>
    {
		private readonly UserManager<ApplicationUser> _userManager;

		public ImgIsOwnerAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
			_userManager = userManager;
        }

		protected override Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			OperationAuthorizationRequirement requirement,
			ImgUpload resource)
		{
			if (context.User == null || resource == null)
				return Task.CompletedTask;
   
			if (requirement.Name == Constants.ReadOperationName)
			{
				context.Succeed(requirement);
			}
			else
			{
				if (requirement.Name == Constants.DeleteOperationName ||
				    requirement.Name == Constants.UpdateOperationName ||
				    requirement.Name == Constants.CreateOperationName)
				{
					if (resource.UserId == _userManager.GetUserId(context.User))
                        context.Succeed(requirement);
				}
			}

			return Task.CompletedTask;
		}
	}
}
