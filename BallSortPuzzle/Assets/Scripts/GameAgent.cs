using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class GameAgent : Agent
{
    private Board _board;
    
    private void Awake()
    {
        _board = GetComponent<Board>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        /*
        foreach (var convertedTube in _board.Tubes.Select(tube => tube.Select(ballColor => (float) ballColor).ToList()))
        {
            sensor.AddObservation(convertedTube);
        }
        */
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        /*
        var discreteActions = actions.DiscreteActions;
        
        // For now, ignores values outside range of tubes ; Ideally, TODO change `Discrete Branch Size` to this value
        if (discreteActions[0] >= _board.Tubes.Count || discreteActions[1] >= _board.Tubes.Count) 
        {
            return;
        }

        if (_board.IsGameOver())
        {
            // Reward
            EndEpisode();
        }
        else if (_board.GetAllMoves().Count == 0)
        {
            // Punish
            EndEpisode();
        }
        else
        {
            if (!_board.CanMove(discreteActions[0], discreteActions[1]))
            {
                // Punish
                return;
            }
            else
            {
                // Reward
            }    
        }
        */
    }

    public override void OnEpisodeBegin()
    {
        //_board.InitializeBoard();
    }
}
