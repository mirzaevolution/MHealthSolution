using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MHealth.Abstracts;
using System.Threading.Tasks;
using AutoMapper;
using MHealth.Api.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using MHealth.DataTransferObjects;

namespace MHealth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        public UserProfilesController(
            IUserProfileService userProfileService,
            IHttpClientFactory httpClientFactory,
            IMapper mapper)
        {
            _userProfileService = userProfileService;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
        }

        [HttpPost(nameof(Register))]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("IdentityServer");

                var registerResult = await httpClient.PostAsync("/account/register", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                return StatusCode((int)registerResult.StatusCode);
            }
            return BadRequest(ModelState);
        }

        [HttpGet(nameof(GetUsersAdminOnly))]
        [ProducesDefaultResponseType(typeof(GetAllUsersPaginatedResponse))]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsersAdminOnly([FromQuery]GetAllUsersPaginatedRequest request)
        {

            var dtoList =  await _userProfileService.GetAll(request.Skip < 0 ? 0 : request.Skip, 
                request.Take < request.Skip ? 100 : request.Take);
            GetAllUsersPaginatedResponse response = new GetAllUsersPaginatedResponse
            {
                Users = _mapper.Map<List<AppUserDto>, List<BaseUserResponse>>(dtoList),
            };
            return Ok(response);
        }

        [HttpGet(nameof(GetById) + "/{id}")]
        [ProducesDefaultResponseType(typeof(GetUserByIdResponse))]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var dto = await _userProfileService.GetById(id);
            GetUserByIdResponse response = new GetUserByIdResponse
            {
                User = _mapper.Map<AppUserDto, BaseUserResponse>(dto)
            };
            return Ok(response);
        }

        [HttpGet(nameof(GetByEmail) + "/{email}")]
        [ProducesDefaultResponseType(typeof(GetUserByEmailResponse))]
        [Authorize]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var dto = await _userProfileService.GetByEmail(email);
            GetUserByEmailResponse response = new GetUserByEmailResponse
            {
                User = _mapper.Map<AppUserDto, BaseUserResponse>(dto)
            };
            return Ok(response);
        }

        [HttpPut(nameof(Update))]
        [ProducesDefaultResponseType(typeof(UpdateUserResponse))]
        [Authorize]

        public async Task<IActionResult> Update([FromBody]UpdateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var updateResult = await _userProfileService.UpdateUser(_mapper.Map<UpdateUserRequest, AppUserDto>(request));
                var response = new UpdateUserResponse
                {
                    User = _mapper.Map<AppUserDto, BaseUserResponse>(updateResult)
                };
                return Ok(response);

            }
            return BadRequest(ModelState);
        }
        
    }
}
