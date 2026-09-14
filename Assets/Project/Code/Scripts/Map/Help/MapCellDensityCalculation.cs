using MapGenearionLibrary.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Code.Scripts.Map.Help
{
    public class MapCellDensityCalculation
    {
        public class MapCellDensityCalculationElement
        {
            public MapPoint Point { get; }
            public float Value { get; set; } = 0.0f;

            public MapCellDensityCalculationElement(MapPoint point) => Point = point;

            public MapCellDensityCalculationElement(int x, int y) => Point = new MapPoint(x, y);

            public void SetValue(float value) => Value = value;

            public float GetValue() => Value;
        }

        public MapCellDensityCalculationElement[] Cells { get; }

        public int Width { get; }

        public int Height { get; }

        public float Factor { get; }

        public float ActiveCellsCount { get; private set; } = 0;

        public float this[MapPoint point]
        {
            get => this[point.X, point.Y];
            private set => this[point.X, point.Y] = value;
        }

        public float this[int x, int y]
        {
            get
            {
                if (Cells.Length == 0)
                {
                    return -1;
                }

                //
                x = x % Width;
                y = y % Height;
                // 
                x = x < 0 ? x + Width : x;
                y = y < 0 ? y + Height : y;

                return Cells[y * Width + x].GetValue();
            }
            private set
            {
                if (Cells.Length == 0)
                {
                    return;
                }

                value = (float)Math.Min(1.0, value);

                //
                x = x % Width;
                y = y % Height;
                // 
                x = x < 0 ? x + Width : x;
                y = y < 0 ? y + Height : y;

                if (value >= 1.0)
                {
                    ActiveCellsCount++;
                }

                Cells[y * Width + x].SetValue(value);
            }
        }

        public MapCellDensityCalculation(MapGenearionLibrary.Map map, float factor = 0.5f)
        {
            Width = map.Width;
            Height = map.Height;
            Factor = (float)Math.Min(1.0, factor);

            Cells = new MapCellDensityCalculationElement[Width * Height];
            foreach (var y in Enumerable.Range(0, Height))
            {
                foreach (var x in Enumerable.Range(0, Width))
                {
                    Cells[y * Width + x] = new MapCellDensityCalculationElement(x, y);
                }
            }
        }

        public void Reset()
        {
            foreach (var cell in Cells)
            {
                cell.SetValue(0.0f);
            }

            ActiveCellsCount = 0;
        }

        public bool IsFullfilled => Width * Height <= ActiveCellsCount;

        public void Set(MapPoint point) => Set(point.X, point.Y);

        public void Set(int x, int y)
        {
            this[x, y] = 1.0f;

            int cellsCount = 0;
            float factor = Factor;
            int startX = x - 2;
            int startY = y - 1;
            int stepIterationsCount = 3;

            while (cellsCount < (Width * Height - 1))
            {
                int stepX = startX;
                int stepY = startY;

                for (int i = 0; i < stepIterationsCount; i++)
                {
                    stepX += 1;

                    this[stepX, stepY] += factor;

                    cellsCount++;
                }

                for (int i = 0; i < stepIterationsCount - 1; i++)
                {
                    stepY += 1;

                    this[stepX, stepY] += factor;

                    cellsCount++;
                }

                for (int i = 0; i < stepIterationsCount - 1; i++)
                {
                    stepX -= 1;

                    this[stepX, stepY] += factor;

                    cellsCount++;
                }

                for (int i = 0; i < stepIterationsCount - 2; i++)
                {
                    stepY -= 1;

                    this[stepX, stepY] += factor;

                    cellsCount++;
                }

                factor *= Factor;
                startX -= 1;
                startY -= 1;
                stepIterationsCount += 2;

                if (factor < 0.0001)
                {
                    break;
                }
            }
        }

        public void Foreach(Action<MapPoint, float> handler)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    handler?.Invoke(new MapPoint(x, y), this[x, y]);
                }
            }
        }

        public IEnumerable<MapPoint> GetMinValueCells()
        {
            if (Cells.Length > 0)
            {
                return Cells.GroupBy(cell => cell.GetValue()).OrderBy(cell => cell.Key).First().Select(cell => cell.Point);
            }
            else
            {
                return null;
            }
        }
    }
}
