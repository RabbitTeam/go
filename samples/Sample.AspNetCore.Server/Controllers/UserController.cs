using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Server.Filter;
using Sample.AspNetCore.Server.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sample.AspNetCore.Server.Controllers
{
    [Route("api/[controller]")]
    [SignatureFilter]
    public class UserController : Controller
    {
        private static readonly ConcurrentDictionary<long, UserModel> Users = new ConcurrentDictionary<long, UserModel>();

        static UserController()
        {
            foreach (var item in Enumerable.Range(1, 10).ToDictionary(i => i, i => new UserModel
            {
                UserName = $"user{i}",
                Age = 20 + i,
                Password = EncodePassword($"password{i}")
            }))
            {
                Users.TryAdd(item.Key, item.Value);
            }

            _id = Users.Count;
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(UserModel), 200)]
        public IActionResult Get(long id)
        {
            if (Users.TryGetValue(id, out var user))
                return Ok(user);

            return NotFound();
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserModel), 200)]
        public IActionResult Get(UserFilter filter)
        {
            IEnumerable<UserModel> users = Users.Values;
            if (filter.MaxAge.HasValue)
                users = users.Where(i => i.Age.HasValue && i.Age.Value <= filter.MaxAge.Value);
            if (filter.MinAge.HasValue)
                users = users.Where(i => i.Age.HasValue && i.Age.Value >= filter.MinAge.Value);
            if (!string.IsNullOrEmpty(filter.UserNameKeyword))
                users = users.Where(i => i.UserName != null && i.UserName.Contains(filter.UserNameKeyword));

            return Ok(users);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(long), 201)]
        public IActionResult Post([FromBody]UserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = GenerateId();

            Users.TryAdd(id, model);

            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public IActionResult Put(int id, [FromBody]UserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Users.TryGetValue(id, out var user))
                return NotFound();

            if (model.Age.HasValue)
                user.Age = model.Age;
            if (!string.IsNullOrEmpty(model.Password))
                user.Password = model.Password;
            if (!string.IsNullOrEmpty(model.UserName))
                user.UserName = model.UserName;

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public IActionResult Delete(int id)
        {
            if (Users.TryRemove(id, out var _))
                return NoContent();

            return NotFound();
        }

        private static int _id;

        private static long GenerateId()
        {
            _id = Interlocked.Increment(ref _id);
            return _id;
        }

        private static string EncodePassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }
    }
}