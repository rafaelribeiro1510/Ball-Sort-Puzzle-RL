using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class GameAgent : Agent
{
    private Camera _mainCamera;
    private Board _board;

    // Rewards
    private int nITers = 0;
    private float nMoves = 0;
    private float nMisses = 0;
    private const float MaxMoves = 5000;
    
    private const float MoveMissReward = -0.01f;
    private const float MoveReward = -0.001f;
    private const float MoveHitReward = 0.01f;
    private const float LossReward = -1f;
    private const float WinReward = 1f;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _board = GetComponent<Board>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (var convertedTube in _board.Tubes.Select(tube => tube.Select(ballColor => (float) ballColor).ToList()))
        {
            sensor.AddObservation(convertedTube);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (nITers > 40) print("STOPOSPTOPOSTPOSTPOOSTPOTPO");
        
        if (heuristicMode)
        {
            if (state != Board.State.MoveMade)
                return;
            state = Board.State.Start;
        }

        var discreteActions = actions.DiscreteActions;
        
        /*
        // For simplified puzzle, ignores values outside range of tubes ; Ideally, TODO change `Discrete Branch Size` to this value
        if (discreteActions[0] >= _board.Tubes.Count || discreteActions[1] >= _board.Tubes.Count) 
        {
            return;
        }
        */
        
        // Try making as few moves as possible
         AddReward(MoveReward);

        if (_board.IsGameOver())
        {
            SetReward(WinFactor());
            // AddReward(WinReward);
            writeToFile(nMoves + " ; " + nMisses); nITers++;
            _board.SetSuccessMat();
            EndEpisode();
        }
        else if (_board.GetAllMoves().Count == 0)
        {
            SetReward(LoseFactor());
            // AddReward(LossReward);
            //print("Locked! " + nMoves + " ; " + nMisses);
            writeToFile(nMoves + " ; " + nMisses); nITers++;
            _board.SetFailureMat();
            EndEpisode();
        }
        else
        {
            if (!_board.CanMove(discreteActions[0], discreteActions[1]))
            {
                // AddReward(MoveMissReward);
                
                nMisses++;
            }
            else
            {
                // Reward moving balls on top of other balls, as opposed to empty tubes <- Proved not effective
                // AddReward(_board.Tubes[discreteActions[1]].Count == 0 ? MoveHitReward : MoveGoodHitReward);
                
                // AddReward(MoveHitReward);

                // Visually move ball, and update board model
                nMoves++;
                RemoveTopBallFromTube(discreteActions[0]);
                PutBallInTheDestinyTube(discreteActions[1]);
            }
        }
    }

    private float WinFactor()
    {
        return 1 - ((nMisses / MaxMoves) * 0.25f) - ((nMoves / MaxMoves) * 0.25f);
    }

    private float LoseFactor()
    {
        return -1f;
    }

    [ContextMenu("New Episode")]
    public override void OnEpisodeBegin()
    {
        nMoves = 0;
        nMisses = 0;
        state = Board.State.Start;
        _board.InitializeBoard();
    }

    private void writeToFile(String str)
    {
        File.AppendAllText("results.txt", str + Environment.NewLine);
    }
    
    
    // Human Interface functions (Heuristic)

    private bool heuristicMode;
    private GameObject ballOutside;
    private Vector3 ballPositionTube;
    private BallColor colorOutside;
    private Board.State state = Board.State.Start;
    private int from, to;
    
    private bool SelectTube(RaycastHit hit) {
        var gameObj = hit.collider.gameObject;
        var tube = gameObj.GetComponent<Tube>();

        if (!gameObj.CompareTag("tube") || tube.isSelected) return false;
        
        tube.ToggleSelected();
        var valueReturn = state == Board.State.Start ? RemoveTopBallFromTube(tube.index) : PutBallInTheDestinyTube(tube.index);
        tube.ToggleSelected();
        if (valueReturn)
        {
            switch (state)
            {
                case Board.State.Start:
                    from = tube.index;
                    break;
                case Board.State.OriginTubeSelected:
                    to = tube.index;
                    break;
            }
        }
        return valueReturn;
    }  

    private bool UnselectedTube(RaycastHit hit) {
        var gameObj = hit.collider.gameObject;
        var tube = gameObj.GetComponent<Tube>();

        if (!gameObj.CompareTag("tube") || !tube.isSelected) return false;
        
        tube.ToggleSelected();
        PutBackBallToTheSameTube(tube.index);
        return true;
    }

    private bool RemoveTopBallFromTube(int index) {
        var tube = _board.TubesScripts[index];

        if(tube.Balls.Count == 0) {
            return false;
        }

        var colorBall = _board.Tubes[index].Pop();
        colorOutside = colorBall;
        
        var ball = tube.RemoveTopBall();

        ballOutside = ball;
        ballPositionTube = ball.transform.localPosition;
        ball.transform.localPosition = new Vector3(ball.transform.localPosition.x, _board.tubeH, ball.transform.localPosition.z);

        return true;
    }

    private void PutBackBallToTheSameTube(int index) {

        var tube = _board.TubesScripts[index];
        tube.Balls.Push(ballOutside);
        _board.Tubes[index].Push(colorOutside);

        ballOutside.transform.localPosition = ballPositionTube;
    }

    private bool PutBallInTheDestinyTube(int index)
    {
        var tube = _board.TubesScripts[index];
        
        if(tube.Balls.Count == _board.tubeH) {
            return false;
        }

        if (_board.Tubes[index].Count > 0 && !_board.Tubes[index].Peek().Equals(colorOutside))
        {
            return false;
        }

        var positionTopBall = tube.GetPositionOfTopBall();
        
        tube.AddBallToTube(ballOutside);
        _board.Tubes[index].Push(colorOutside);
        ballOutside.transform.localPosition = new Vector3(positionTopBall.x, positionTopBall.y+1, positionTopBall.z);
        ballOutside = null;

        return true;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        heuristicMode = true;
        
        actionsOut.DiscreteActions.Clear();
        
        if (state == Board.State.GameOver) 
            return;
        
        if (Input.GetMouseButtonDown(0)) {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f)) {
                switch(state) {
                    case Board.State.Start:
                        if (SelectTube(hit)) {
                            print("Selected");
                            state = Board.State.OriginTubeSelected;
                        }
                        break;
                    case Board.State.OriginTubeSelected:
                        if (UnselectedTube(hit)) {
                            print("Put back");
                            state = Board.State.MoveMade;
                        }
                        if (SelectTube(hit)) {
                            print("Moved!");
                            state = Board.State.MoveMade;
                            var discreteActions = actionsOut.DiscreteActions; 
                            discreteActions[0] = from;
                            discreteActions[1] = to;
                        }
                        break;
                } 
            }
        }
    }
}
