using NUnit.Framework;
using System.Threading.Tasks;
using TestHelpers;
using System.Linq;

namespace DataProcessor.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        [Test]
        public async void Output_WhenAllowedToComplete_IsIterations()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;

            // Act
            viewModel.StartProcess();
            await Task.Delay(600);

            // Assert
            Assert.AreEqual(viewModel.Iterations.ToString(), viewModel.Output);
        }

        [Test]
        public async void Output_OnCanceled_IsCanceled()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;

            // Act
            viewModel.StartProcess();
            await Task.Delay(100);
            viewModel.CancelProcess();
            await Task.Delay(100);

            // Assert
            Assert.AreEqual("Canceled", viewModel.Output);
        }


        [Test]
        public void StartEnabled_BeforeStarted_IsEnabled()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;

            // Act
            // Don't do anything

            // Assert
            Assert.IsTrue(viewModel.StartEnabled);
        }

        [Test]
        public async void StartEnabled_WhenRunning_IsDisabled()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;

            // Act
            viewModel.StartProcess();
            await Task.Delay(100);

            // Assert
            Assert.IsFalse(viewModel.StartEnabled);
        }

        [Test]
        public async void StartEnabled_WhenCompleted_IsEnabled()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;

            // Act
            viewModel.StartProcess();
            await Task.Delay(600);
            Assert.AreEqual("5", viewModel.Output);

            // Assert
            Assert.IsTrue(viewModel.StartEnabled);
        }


        [Test]
        public void CancelEnabled_BeforeStarted_IsDisabled()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;

            // Act
            // Don't do anything

            // Assert
            Assert.IsFalse(viewModel.CancelEnabled);
        }

        [Test]
        public async void CancelEnabled_WhenRunning_IsEnabled()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;

            // Act
            viewModel.StartProcess();
            await Task.Delay(100);

            // Assert
            Assert.IsTrue(viewModel.CancelEnabled);
        }

        [Test]
        public async void CancelEnabled_WhenCompleted_IsDisabled()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;

            // Act
            viewModel.StartProcess();
            await Task.Delay(600);
            Assert.AreEqual("5", viewModel.Output);

            // Assert
            Assert.IsFalse(viewModel.CancelEnabled);
        }

        [Test]
        public async void Progress_WhenCompleted_IsUpdatedIterationsPlusOneTimes()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;
            int expectedProgressCount = viewModel.Iterations + 1;
            var tracker = new PropertyChangeTracker(viewModel);

            // Act
            tracker.Reset();
            viewModel.StartProcess();
            await Task.Delay(600);

            var progressCount = 
                tracker.ChangeCount(nameof(viewModel.ProgressPercentage));

            // Assert
            Assert.AreEqual(expectedProgressCount, progressCount);
        }

        [Test]
        public async void Progress_WhenCanceled_IsUpdatedLessThanIterations()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.Iterations = 5;
            int expectedProgressCount = viewModel.Iterations + 1;
            var tracker = new PropertyChangeTracker(viewModel);

            // Act
            tracker.Reset();
            viewModel.StartProcess();
            await Task.Delay(200);
            viewModel.CancelProcess();
            await Task.Delay(100);

            var progressCount =
                tracker.ChangeCount(nameof(viewModel.ProgressPercentage));

            // Assert
            Assert.Less(progressCount, expectedProgressCount);
        }
    }
}
