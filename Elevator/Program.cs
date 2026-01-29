using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elevator
{
    internal class Program
    {
        // ENUMS for clarity and strong typing (OOP Pillar: Abstraction)
        public enum Direction { Idle, Up, Down }
        public enum DoorState { Open, Closed }

        public class OElevator
        {
            //Properties like CurrentFloor have a private set.The elevator's state can only be altered
            //by its own methods (MoveTo, StopAtFloor), guaranteeing data integrity.
            public int CurrentFloor { get; private set; } = 1;
            public Direction CurrentDirection { get; private set; } = Direction.Idle;
            public DoorState Doors { get; private set; } = DoorState.Closed;

            //I used a SortedSet because it avoids duplicate stops and keeps floors sorted, which makes it easier
            //to optimize the elevator route(Scanning Algorithm)
            private readonly SortedSet<int> _stops = new SortedSet<int>();

            private const int MaxFloor = 5;
            private const int MinFloor = 1;

            // Main method for requesting the elevator
            public void RequestFloor(int floor)
            {
                if (floor < MinFloor || floor > MaxFloor) return;
                _stops.Add(floor);
                UpdateDirection();
            }

            // Motion simulation
            public void ProcessNextStop()
            {
                if (!_stops.Any())
                {
                    CurrentDirection = Direction.Idle;
                    return;
                }

                // Determine next stop based on current direction
                int nextFloor = CurrentDirection == Direction.Down
                    ? _stops.Reverse().FirstOrDefault(f => f < CurrentFloor)
                    : _stops.FirstOrDefault(f => f > CurrentFloor);

                // If there are no more in that direction, take the nearest one from the set.
                if (nextFloor == 0) nextFloor = _stops.First();

                MoveTo(nextFloor);
            }

            private void MoveTo(int floor)
            {
                Console.WriteLine($"\n[Closing Doors]");
                Doors = DoorState.Closed;

                while (CurrentFloor != floor)
                {
                    if (CurrentFloor < floor) { CurrentFloor++; CurrentDirection = Direction.Up; }
                    else { CurrentFloor--; CurrentDirection = Direction.Down; }

                    Console.WriteLine($"Elevator on floor: {CurrentFloor}...");
                    Thread.Sleep(500); // Time simulation
                }

                StopAtFloor(floor);
            }

            private void StopAtFloor(int floor)
            {
                CurrentDirection = Direction.Idle;
                Doors = DoorState.Open;
                _stops.Remove(floor);
                Console.WriteLine($"[We arrived at the floor {floor}. Open Doors]");
                UpdateDirection();
            }

            private void UpdateDirection()
            {
                if (!_stops.Any()) CurrentDirection = Direction.Idle;
                else CurrentDirection = _stops.First() > CurrentFloor ? Direction.Up : Direction.Down;
            }

        }

        static void Main(string[] args)
        {
            OElevator elevator = new OElevator();
            bool exit = false;

            Console.WriteLine("=== ELEVATOR SIMULATOR ===");
            Console.WriteLine("Available floors 1 - 5. (Write 'EXIT' to  finish)");

            while (!exit)
            {
                Console.Write("\n¿What floor neeed? (1-5): ");
                string input = Console.ReadLine();

                // 1. Opción de salida
                if (input?.ToLower() == "EXIT")
                {
                    exit = true;
                    continue;
                }

                // 2. Validación: Intentar convertir a entero (TryParse es mejor que Parse)
                if (int.TryParse(input, out int selectedFloor))
                {
                    // 3. Validación de rango (Regla de negocio)
                    if (selectedFloor >= 1 && selectedFloor <= 5)
                    {
                        elevator.RequestFloor(selectedFloor);

                        // Procesar el movimiento hasta que el ascensor se detenga
                        while (elevator.CurrentDirection != Direction.Idle || elevator.Doors == DoorState.Closed)
                        {
                            elevator.ProcessNextStop();
                            if (elevator.CurrentDirection == Direction.Idle) break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("(!) Error: The floor must be between 1 and 5.");
                    }
                }
                else
                {
                    // 4. Manejo de entradas no numéricas (letras o vacíos)
                    Console.WriteLine("(!) Error: Invalid entry. Please enter a number.");
                }
            }

            Console.WriteLine("Simulation completed.");
        }
    }
}
