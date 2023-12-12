using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using WebApiMongoPOC.Controllers;
using WebApiMongoPOC.Models;
using WebApiMongoPOC.Models.DTOs;
using WebApiMongoPOC.Services;

namespace WebApiMongoPOC.Test
{
    public class PlayListTests
    {
        private PlayListController _controller;
        private IFixture _fixture;
        private Mock<IPlayListService> _mockMongoDBService;
        private List<PlayListDTO> _playLists;
        private Mock<IMapper> _mapper;

        public PlayListTests() 
        {
            _fixture = new Fixture();
            _mapper = new Mock<IMapper>();
            _mockMongoDBService = new Mock<IPlayListService>();
            _fixture.Register(() => _mockMongoDBService.Object);
            _playLists = _fixture.CreateMany<PlayListDTO>().ToList();
            _controller = new PlayListController(_mockMongoDBService.Object);
        }

        [Fact]
        public async void Get_PlayLists_ReturnsPlayLists()
        {
            _mockMongoDBService.Setup(m => m.GetPlayListsAsync()).ReturnsAsync(_playLists);

            var result = await _controller.Get();

            var objectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var actualValue = objectResult.Value.Should().BeOfType<List<PlayListDTO>>().Subject;
            actualValue.Should().NotBeNull();
            actualValue.Should().Equal(_playLists);
        }

        [Fact]
        public async void Get_PlayListById_ReturnsPlayLists()
        {
            var playList = _playLists.FirstOrDefault();
            var playListID = _playLists.FirstOrDefault().Id;
            _mockMongoDBService.Setup(m => m.GetPlayListByIdAsync(playListID)).ReturnsAsync(playList);

            var result = await _controller.GetById(playListID);

            var objectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var actualValue = objectResult.Value.Should().BeOfType<PlayListDTO>().Subject;
            actualValue.Should().NotBeNull();
            actualValue.Should().BeEquivalentTo(playList);
        }

        [Fact]
        public async void Add_PlayList_ReturnsCreated()
        {
            PlayList playList = _fixture.Create<PlayList>();
            var playListID = playList.Id;
            _mockMongoDBService.Setup(m => m.CreatePlayListAsync(playList)).ReturnsAsync(playListID);

            var result = await _controller.Post(playList);

            var objectResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var actualValue = objectResult.Value.Should().BeOfType<PlayList>().Subject;
            actualValue.Should().NotBeNull();
            actualValue.Should().BeEquivalentTo(playList);
        }

        [Fact]
        public async void Update_PlayList_ReturnsNoContent()
        {
            PlayListDTO playListDTO = _playLists.FirstOrDefault();
            PlayList playList = new PlayList() { Id = playListDTO.Id, name = playListDTO.name, movies = playListDTO.movies };
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
        public async void Get_InvalidPlaylist_ReturnsNotFound()
        {
            _mockMongoDBService.Setup(m => m.GetPlayListByIdAsync(It.IsAny<string>())).Throws(new InvalidOperationException("PlayList not found"));

            var result = await _controller.GetById(It.IsAny<string>());

            var actualValue = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            actualValue.Value.Should().Be("PlayList not found");
        }

        [Fact]
        public async void Update_InvalidPlaylist_ReturnsBadRequest()
        {
            var playList = _fixture.Create<PlayList>();
            _mockMongoDBService.Setup(m => m.UpdatePlayListAsync(playList)).Throws(new InvalidOperationException("Failed to update playlist"));

            var result = await _controller.Update(playList);

            var actualValue = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            actualValue.Value.Should().Be("Failed to update playlist");
        }

        [Fact]
        public async void Delete_InvalidPlaylist_ReturnsNotFound()
        {
            var playListID = new Guid().ToString();
            _mockMongoDBService.Setup(m => m.DeleteAsync(playListID)).Throws(new InvalidOperationException("Failed to delete playlist"));

            var result = await _controller.Delete(playListID);

            var actualValue = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            actualValue.Value.Should().Be("Failed to delete playlist");
        }
    }
}