using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using WebApiMongoPOC.Controllers;
using WebApiMongoPOC.Models;
using WebApiMongoPOC.Services;

namespace WebApiMongoPOC.Test
{
    public class PlayListTests
    {
        private PlayListController _controller;
        private IFixture _fixture;
        private Mock<IMongoDBService> _mockMongoDBService;
        private List<PlayList> _playLists;

        public PlayListTests() 
        {
            _fixture = new Fixture();
            _mockMongoDBService = new Mock<IMongoDBService>();
            _fixture.Register(() => _mockMongoDBService.Object);
            _playLists = _fixture.CreateMany<PlayList>().ToList();
            _controller = new PlayListController(_mockMongoDBService.Object);
        }

        [Fact]
        public async void Get_PlayLists_ReturnsPlayLists()
        {
            _mockMongoDBService.Setup(m => m.GetPlayListsAsync()).ReturnsAsync(_playLists);

            var result = await _controller.Get();

            var objectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var actualValue = objectResult.Value.Should().BeOfType<List<PlayList>>().Subject;
            actualValue.Should().NotBeNull();
            actualValue.Should().Equal(_playLists);
        }

        [Fact]
        public async void Add_PlayList_ReturnsCreated()
        {
            PlayList playList = _fixture.Create<PlayList>();
            _mockMongoDBService.Setup(m => m.CreatePlayListAsync(playList)).Returns(Task.CompletedTask);

            var result = await _controller.Post(playList);

            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async void Update_PlayList_ReturnsNoContent()
        {
            var playList = _playLists.FirstOrDefault();
            playList.name = "Drama";
            _mockMongoDBService.Setup(m => m.UpdatePlayListAsync(playList)).Returns(Task.CompletedTask);

            var result = await _controller.Update(playList);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async void Delete_PlayList_ReturnsNoContent()
        {
            var playListID = _playLists.FirstOrDefault().Id;
            _mockMongoDBService.Setup(m => m.DeleteAsync(playListID)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(playListID);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async void Add_InvalidName_ReturnsValidationError()
        {
            _controller.ModelState.AddModelError("name", "The field name must be a string or array type with a minimum length of '3'.");
            _fixture.Customize<PlayList>(x => x.With(x => x.name, "AB"));
            var playList = _fixture.Create<PlayList>();

            var result = await _controller.Post(playList);

            var actualValue = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            ((ModelStateDictionary.ValueEnumerable)actualValue.Value).AsEnumerable().ToList()[0].Errors[0].ErrorMessage.Should().Be("The field name must be a string or array type with a minimum length of '3'.");
        }

        [Fact]
        public async void Add_InvalidMovies_ReturnsValidationError()
        {
            _controller.ModelState.AddModelError("movies", "The field movies must be a string or array type with a minimum length of '1'.");
            _fixture.Customize<PlayList>(x => x.With(x => x.movies, new List<string>()));
            var playList = _fixture.Create<PlayList>();

            var result = await _controller.Post(playList);

            var actualValue = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            ((ModelStateDictionary.ValueEnumerable)actualValue.Value).AsEnumerable().ToList()[0].Errors[0].ErrorMessage.Should().Be("The field movies must be a string or array type with a minimum length of '1'.");
        }

        [Fact]
        public async void Update_InvalidPlaylist_ReturnsNotFound()
        {
            var playList = _fixture.Create<PlayList>();
            _mockMongoDBService.Setup(m => m.UpdatePlayListAsync(playList)).Throws(new KeyNotFoundException());

            var result = await _controller.Update(playList);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void Delete_InvalidPlaylist_ReturnsNotFound()
        {
            var playListID = new Guid().ToString();
            _mockMongoDBService.Setup(m => m.DeleteAsync(playListID)).Throws(new KeyNotFoundException());

            var result = await _controller.Delete(playListID);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}