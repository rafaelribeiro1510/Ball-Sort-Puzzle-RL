<!--
Copyright (C) 2021 Rafael Ribeiro, Diogo Rodrigues, Bernardo Ferreira
Distributed under the terms of the GNU General Public License, version 3
-->

# Ball Sort Puzzle with reinforcement learning

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![License: CC BY-NC-ND 4.0](https://img.shields.io/badge/License-CC%20BY--NC--ND%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by-nc-nd/4.0/)

[![report1](https://github.com/up201806330/iart-proj-2/actions/workflows/report1.yml/badge.svg)](https://github.com/up201806330/iart-proj-2/actions/workflows/report1.yml)
[![report2](https://github.com/up201806330/iart-proj-2/actions/workflows/report2.yml/badge.svg)](https://github.com/up201806330/iart-proj-2/actions/workflows/report2.yml)


- **Project name:** Ball Sort Puzzle
- **Short description:** Solve the [Ball Sort Puzzle](https://play.google.com/store/apps/details?id=com.spicags.ballsort&hl=pt_PT&gl=US) using reinforcement learning
- **Environment:** Unity
- **Tools:** C#, reinforcement learning
- **Institution:** [FEUP](https://sigarra.up.pt/feup/en/web_page.Inicial)
- **Course:** [IART](https://sigarra.up.pt/feup/en/UCURR_GERAL.FICHA_UC_VIEW?pv_ocorrencia_id=459487) (Artificial Intelligence)
- **Project grade:** 19.5/20.0
- **Group members:**
    - [Diogo Miguel Ferreira Rodrigues](https://github.com/dmfrodrigues) (<dmfrodrigues2000@gmail.com> / <diogo.rodrigues@fe.up.pt>)
    - [Rafael Soares Ribeiro](https://github.com/up201806330) (<up201806330@fe.up.pt>)
    - [Bernardo António Magalhães Ferreira](https://github.com/BernardoFerreira00) (<up201806581@fe.up.pt>)

Based on game [*Ball Sort Puzzle*](https://play.google.com/store/apps/details?id=com.spicags.ballsort&hl=pt_PT&gl=US) by [Spica Game Studio](https://play.google.com/store/apps/developer?id=Spica+Game+Studio).

## Installing and running

1. Open `BallSortPuzzle` folder as a Unity project (v. 2020.2.2f1)
2. If scene is empty, open the `Scene` file, in the `Scenes` folder
3. Select the `Board` GameObject in the hierarchy, and in the `Behaviour Parameters` script, select a model brain to test.
4. Make sure the `Behaviour Type` is set to `Inference Only` (this is so that ml-agents uses the model to make decisions, instead of training by default)
5. Click play to see the model play some random, simple ball sort puzzles. After every game, a new one of the same proportions and difficulty is generated

(To manually play the game, select `Heuristic Only` in the `Behaviour Type`, play with the mouse, selecting the tubes to move the ball from and to with `LMB`. Note that due to the way ml-agents polls for input, some mouse inputs are lost)