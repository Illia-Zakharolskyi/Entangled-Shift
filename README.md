# Entangled-Shift
## Final project (developed throughout the entire 3rd semester with a team of 4 people)

---

## Freedom: Washed ashore on an unknown coast following a disaster, you find yourself facing a choice: seek a way back to your own world, or become the absolute ruler of this island? Your fate is in your hands.

---

## Gameplay: will you run away or will you be the ruler, you will encounter challenges:
- think, build, farm
- survive the nights
- and...
```diff
- DON'T GET SCARED!
```
---

## 🛠 Tech Stack

- **Language**: c#
- **Environment**: Unity Hub, Unity Editor(6.5), Microsoft Visual Studio, GitHub Desktop
- **Libraries & Tools**: unknown(will be known in the future)

---

## 🚀 How to Run

1. Download and install Unity Hub and Unity Editor(6.5 recommended)
2. Clone the repository to your computer using one of the methods below:
   - **Using Git (Recommended):** Open your terminal/command prompt and run:
     ```bash
     git clone https://github.com/Illia-Zakharolskyi/Entangled-Shift
     ```
   - **Manual Download:** Click the green **Code** button on GitHub, select **Download ZIP**, and extract it to a folder of your choice.
3. Open Unity Hub, click **Add** in the top-right corner, select **Add project from disk**, and choose the folder containing the cloned or downloaded project.
4. Open the project in Unity Editor and let it load all packages and assets.

### 💡 Important Note:
If you want to run any scene, it is highly recommended to launch it from the **"Bootstrap"** scene. 

To load a specific scene:
1. Open the **"Bootstrap"** scene.
2. Select the **`BootstrapLoader`** GameObject.
3. In the Inspector, set the **`Name`** property of the `BootstrapLoader` script to your target scene's name.

> **Why?** Many scenes rely on managers and scripts initialized during the bootstrap phase. Launching scenes directly may cause missing reference errors.
