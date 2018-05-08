﻿using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Presentation.Authorization
{
    public static class ImgUploadOperations
    {
		public static OperationAuthorizationRequirement Create =
			new OperationAuthorizationRequirement{ Name = Constants.CreateOperationName };

		public static OperationAuthorizationRequirement Read =
          new OperationAuthorizationRequirement { Name = Constants.ReadOperationName };
		
        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement { Name = Constants.UpdateOperationName };
        
		public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
    }   
}
