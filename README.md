# iart-proj-2

## Installing and running

1. Open `BallSortPuzzle` folder as a Unity project (v. 2020.2.2f1)
2. If scene is empty, open the `Scene` file, in the `Scenes` folder
3. Select the `Board` GameObject in the hierarchy, and in the `Behaviour Parameters` script, select a model brain to test.
4. Make sure the `Behaviour Type` is set to `Inference Only` (this is so that ml-agents uses the model to make decisions, instead of training by default)
5. Click play to see the model play some random, simple ball sort puzzles. After every game, a new one of the same proportions and difficulty is generated