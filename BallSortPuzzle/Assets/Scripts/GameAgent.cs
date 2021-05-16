using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class GameAgent : Agent
{
    private Camera _mainCamera;
    private Board _board;

    // Rewards
    private const float MoveMissReward = -0.1f;
    private const float MoveHitReward = 2f;
    private const float LossReward = -5f;
    private const float WinReward = 50f;

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
        if (state != Board.State.MoveMade)
            return;
        state = Board.State.Start;

        var discreteActions = actions.DiscreteActions;
        print(discreteActions[0] + " ; " + discreteActions[1]);
        
        // For now, ignores values outside range of tubes ; Ideally, TODO change `Discrete Branch Size` to this value
        if (discreteActions[0] >= _board.Tubes.Count || discreteActions[1] >= _board.Tubes.Count) 
        {
            return;
        }

        if (_board.IsGameOver())
        {
            AddReward(WinReward);
            _board.SetSuccessMat();
            //EndEpisode();
        }
        else if (_board.GetAllMoves().Count == 0)
        {
            AddReward(LossReward);
            _board.SetFailureMat();
            //EndEpisode();
        }
        else
        {
            if (!_board.CanMove(discreteActions[0], discreteActions[1]))
            {
                AddReward(MoveMissReward);
            }
            else
            {
                AddReward(MoveHitReward);
                
                // Visually move ball, and update board model
                RemoveTopBallFromTube(discreteActions[0]);
                PutBallInTheDestinyTube(discreteActions[1]);
            }    
        }
    }

    [ContextMenu("New Episode")]
    public override void OnEpisodeBegin()
    {
        print("New ep!");
        state = Board.State.Start;
        _board.InitializeBoard();
    }

    // Human Interface functions (Heuristic)
    
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
        ballPositionTube = ball.transform.position;
        ball.transform.position = new Vector3(ball.transform.position.x, _board.tubeH, ball.transform.position.z);

        return true;
    }

    private void PutBackBallToTheSameTube(int index) {

        var tube = _board.TubesScripts[index];
        tube.Balls.Push(ballOutside);
        _board.Tubes[index].Push(colorOutside);

        ballOutside.transform.position = ballPositionTube;
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
        ballOutside.transform.position = new Vector3(positionTopBall.x, positionTopBall.y+1, positionTopBall.z);
        ballOutside = null;

        return true;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
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
