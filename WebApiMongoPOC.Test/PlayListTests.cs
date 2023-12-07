using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
            _playLists = _fixture.CreateMany<PlayList>(3).ToList();
            _mockMongoDBService.Setup(m => m.GetPlayListsAsync()).ReturnsAsync(_playLists);
            _mockMongoDBService.Setup(m => m.CreatePlayListAsync(It.IsAny<PlayList>()));
            _mockMongoDBService.Setup(m => m.UpdatePlayListAsync(It.IsAny<PlayList>()));
            _mockMongoDBService.Setup(m => m.DeleteAsync(It.IsAny<string>()));
            _controller = new PlayListController(_mockMongoDBService.Object);
        }

        [Fact]
        public async void Get_PlayLists_ReturnsPlayLists()
        {
            var result = await _controller.Get();

            var objectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(200);
            var actualValue = objectResult.Value.Should().BeOfType<List<PlayList>>().Subject;
            actualValue.Should().NotBeNull();
            actualValue.Should().HaveCount(3);
        }

        [Fact]
        public async void Add_PlayList_ReturnsCreated()
        {
            PlayList playList = _fixture.Create<PlayList>();

            var result = await _controller.Post(playList);

            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async void Update_PlayList_ReturnsNoContent()
        {
            var playList = _playLists.FirstOrDefault();
            playList.name = "Drama";

            var result = await _controller.Update(playList);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async void Delete_PlayList_ReturnsNoContent()
        {
            var playListID = _playLists.FirstOrDefault().Id;
            
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

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async void Add_InvalidMovies_ReturnsValidationError()
        {
            _controller.ModelState.AddModelError("movies", "The field movies must be a string or array type with a minimum length of '1'.");
            _fixture.Customize<PlayList>(x => x.With(x => x.movies, new List<string>()));
            var playList = _fixture.Create<PlayList>();

            var result = await _controller.Post(playList);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async void Update_InvalidPlaylist_ReturnsNotFound()
        {
            _mockMongoDBService.Setup(m => m.UpdatePlayListAsync(It.IsAny<PlayList>())).Throws(new KeyNotFoundException());
            var playList = _fixture.Create<PlayList>();

            var result = await _controller.Update(playList);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void Delete_InvalidPlaylist_ReturnsNotFound()
        {
            _mockMongoDBService.Setup(m => m.DeleteAsync(It.IsAny<string>())).Throws(new KeyNotFoundException());
            var playListID = _playLists.FirstOrDefault().Id;

            var result = await _controller.Delete(playListID);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}