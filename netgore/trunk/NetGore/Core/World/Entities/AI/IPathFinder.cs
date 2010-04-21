﻿using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;

namespace NetGore.AI
{
    public interface IPathFinder
    {
        Heuristics HeuristicFormula { get; set; }

        IEnumerable<Node> FindPath(Vector2 Start, Vector2 End);
    }
}