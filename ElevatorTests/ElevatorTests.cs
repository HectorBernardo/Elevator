using Xunit;
using static ElevatorSystem.Program; // Asegúrate que coincida con tu namespace

namespace ElevatorTests
{
    public class ElevatorTests
    {
        [Fact]
        public void InitialState_ShouldBeFloorOneAndClosed()
        {
            // Arrange & Act
            var elevator = new Elevator();

            // Assert
            Assert.Equal(1, elevator.CurrentFloor);
            Assert.Equal(DoorState.Closed, elevator.Doors);
            Assert.Equal(Direction.Idle, elevator.CurrentDirection);
        }

        [Fact]
        public void RequestFloor_ShouldMoveToRequestedFloor()
        {
            // Arrange
            var elevator = new Elevator();
            int targetFloor = 3;

            // Act
            elevator.RequestFloor(targetFloor);
            elevator.ProcessNextStop();

            // Assert
            Assert.Equal(targetFloor, elevator.CurrentFloor);
            Assert.Equal(DoorState.Open, elevator.Doors);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(0)]
        [InlineData(-1)]
        public void RequestFloor_OutsideRange_ShouldIgnoreRequest(int invalidFloor)
        {
            // Arrange
            var elevator = new Elevator();

            // Act
            elevator.RequestFloor(invalidFloor);
            elevator.ProcessNextStop();

            // Assert
            // El ascensor debe ignorar el comando y quedarse en el piso 1
            Assert.Equal(1, elevator.CurrentFloor);
        }

        [Fact]
        public void MultipleRequests_ShouldProcessInOrder()
        {
            // Arrange
            var elevator = new Elevator();

            // Act
            elevator.RequestFloor(5);
            elevator.RequestFloor(2);

            // Primera parada (Piso 2 por estar en camino al 5 o por ser el primero ordenado)
            elevator.ProcessNextStop();
            int firstStop = elevator.CurrentFloor;

            // Segunda parada
            elevator.ProcessNextStop();
            int secondStop = elevator.CurrentFloor;

            // Assert
            Assert.Equal(2, firstStop);
            Assert.Equal(5, secondStop);
        }
    }
}