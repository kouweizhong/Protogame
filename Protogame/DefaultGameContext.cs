using System;
using Microsoft.Xna.Framework;
using Ninject;

namespace Protogame
{
    class DefaultGameContext : IGameContext
    {
        private IKernel m_Kernel;

        public GraphicsDeviceManager Graphics { get; set; }
        public IWorld World { get; set; }
        public GameTime GameTime { get; set; }
        public Camera Camera { get; set; }
        public IGameWindow Window { get; set; }
        public int FPS { get; set; }
        public Game Game { get; internal set; }
        public IWorldManager WorldManager { get; internal set; }
        public int FrameCount { get; set; }
        
        public DefaultGameContext(
            IKernel kernel,
            Game game,
            GraphicsDeviceManager graphics,
            IGameWindow window,
            IWorld world,
            IWorldManager worldManager)
        {
            this.m_Kernel = kernel;
            this.Game = game;
            this.Graphics = graphics;
            this.World = world;
            this.WorldManager = worldManager;
            this.Window = window;
        }
        
        public void ResizeWindow(int width, int height)
        {
            if (this.Window.ClientBounds.Width == width &&
                this.Window.ClientBounds.Height == height)
                return;
            this.Graphics.PreferredBackBufferWidth = width;
            this.Graphics.PreferredBackBufferHeight = height;
            this.Camera = new Camera(width, height);
            this.Graphics.ApplyChanges();
        }

        public IWorld CreateWorld<T>() where T : IWorld
        {
            return this.m_Kernel.Get<T>();
        }

        public IWorld CreateWorld<TFactory>(Func<TFactory, IWorld> creator)
        {
            return creator(this.m_Kernel.Get<TFactory>());
        }
        
        public void SwitchWorld<T>() where T : IWorld
        {
            if (this.World != null)
                this.World.Dispose();
            this.World = this.CreateWorld<T>();
        }

        public void SwitchWorld<TFactory>(Func<TFactory, IWorld> creator)
        {
            if (this.World != null)
                this.World.Dispose();
            this.World = this.CreateWorld<TFactory>(creator);
        }
        
        public void SwitchWorld<T>(T world) where T : IWorld
        {
            if (this.World != null)
                this.World.Dispose();
            this.World = world;
        }
    }
}
