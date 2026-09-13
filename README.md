<p align="center">
  <img src="logo/logo_label.png" />
</p>
<p align="center">
  <i>"Rubedo is a Latin word meaning "redness" that was adopted by alchemists to define the fourth and final major stage in their magnum opus - Rubedo signaled alchemical success, and the end of the great work."</i>
</p>

##
Rubedo is a game engine based on [Monogame](https://monogame.net/), with the goal of being extremely easy to work in while remaining completely open source. (That is, you'll never have to pay anyone to use any part of this!)
It tries to be very Unity-like in structure, although deviates severely for several systems because I hate how Unity does certain things.
Still, if you've used Unity before, a good chunk of this should be a little familiar.

 **[Documentation](docs/README.md)**

# What does it do?
Everything Monogame does, and more:
  - Entity-Component system (NOT ECS, IT'S EC)
  - Basic 2D physics
  - Particles
  - Automatic content compilation
  - Auto texture packing
  - Input management
  - Coroutines
  - Audio System using SoLoud
  - Animation systems
  - A lot more things that I can't think of right now!

# How do I do?
To install, you'll need to do the following:
  1. Install [Visual Studio](https://visualstudio.microsoft.com/)
  2. Install [Monogame's package](https://docs.monogame.net/articles/tutorials/building_2d_games/02_getting_started/index.html) and create a new Cross-Platform Monogame project.
  3. Download and add the [Rubedo package](https://github.com/Sirplop/RubedoEngine/tree/master/Rubedo) to your project.
  4. Make your `Game1.cs` (or whatever you're calling your game class) subclass `RubedoEngine`.
  5. Download the latest release of the [Content Compiler](https://github.com/Sirplop/Rubedo.Compiler/releases/tag/Release) and put it in a folder called `ContentBuilder` inside of your project folder.
  6. Open your game's .csproj, and add the following:
       ```csproj
        <Target Name="BuildContent" AfterTargets="PostBuildEvent">
          <Exec Command="Rubedo.Compiler.exe $(ProjectDir) $(TargetDir) textures" WorkingDirectory="$(ProjectDir)\ContentBuilder" />
        </Target>
        
        <PropertyGroup>
          <DisableFastUpToDateCheck>true</DisableFastUpToDateCheck>
        </PropertyGroup>
      ```
  7. Go read the documentation and have fun!

Hopefully all of that will one day be relegated to a script to do it for you, but for now you'll have to make do with the dreaded _manual work!_

# Can I do?
Of course! It's MIT licenced, and no library it uses have a restrictive licence, so you are completely free to do whatever you want with it! (an attribution is always nice, but not necessary!)

# Library Attribution
- [MonoGame](https://monogame.net/)
- [FontStashSharp](https://github.com/Sirplop/FontStashSharp)
- [NLog](https://nlog-project.org/)
- [SoLoud](https://github.com/jarikomppa/soloud)
- [FiniteStateMachine](https://github.com/UnterrainerInformatik/FiniteStateMachine)
- [MonoGame.Particles](https://github.com/SjaakAlvarez/MonoGame.Particles)
- [SkiaSharp](https://github.com/mono/SkiaSharp)
- [AsepriteDotNet](https://github.com/AristurtleDev/AsepriteDotNet)
