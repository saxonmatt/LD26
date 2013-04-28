﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LudumDare26
{
    class PromptController
    {
        public enum PromptType
        {
            Text,
            Image
        }

        public class Prompt
        {
            public PromptType Type;
            public string Name;
            public string Text;
            public bool IsActive;
            public double Delay;
            public bool IsTimed;
            public double Time;
            public float Alpha;
            public bool HasDisplayed = false;
        }

        public static PromptController Instance;

        SpriteFont font;

        List<Prompt> prompts = new List<Prompt>();

        Dictionary<string, Texture2D> promptImages = new Dictionary<string, Texture2D>();

        public PromptController()
        {
            Instance = this;

        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("font");


        }

        public void Update(GameTime gameTime)
        {
            foreach (Prompt p in prompts)
            {
                if (p.IsActive)
                {
                    p.HasDisplayed = true;

                    p.Alpha = MathHelper.Lerp(p.Alpha, 1f, 0.05f);
                    if (p.IsTimed)
                    {
                        p.Time -= gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (p.Time <= 0) p.IsActive = false;
                    }
                }
                else
                {
                    if (p.Delay > 0 && !p.HasDisplayed)
                    {
                        p.Delay -= gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (p.Delay <= 0) p.IsActive = true;
                    }
                    p.Alpha = MathHelper.Lerp(p.Alpha, 0f, 0.1f);
                }
                               
            }
        }

        public void Draw(GraphicsDevice gd, SpriteBatch sb)
        {
            Vector2 pos = new Vector2(gd.Viewport.Bounds.Center.X, (gd.Viewport.Bounds.Center.Y / 2)-100f);
            foreach (Prompt p in prompts)
            {
                if (p.Alpha > 0.05f)
                {
                    switch (p.Type)
                    {
                        case PromptType.Text:
                            Vector2 size = font.MeasureString(p.Text);
                            ShadowText(sb, p.Text, pos, Color.Salmon * p.Alpha, size / 2, 1f);
                            pos.Y += (size.Y-5);
                            break;
                        case PromptType.Image:
                            break;
                    }
                }
            }
        }

        public void AddPrompt(string name, PromptType type, string text, bool isTimed, double time, double delay)
        {
            if (prompts.Find(p => p.Name == name) == null)
            {
                prompts.Add(new Prompt()
                {
                    Name = name,
                    Type = type,
                    Text = text,
                    Delay = delay,
                    IsTimed = isTimed,
                    Time = time,
                    IsActive = (delay>0)?false:true,
                    Alpha = 0f,
                    HasDisplayed = false
                });
            }
        }

        public void RemovePrompt(string name)
        {
            try
            {
                prompts.First(p => p.Name == name).IsActive = false;
                prompts.First(p => p.Name == name).HasDisplayed = true;
            }
            catch (Exception ex) { }
        }

        void ShadowText(SpriteBatch sb, string text, Vector2 pos, Color col, Vector2 off, float scale)
        {
            sb.DrawString(font, text, pos + (Vector2.One * 2f), new Color(0, 0, 0, col.A), 0f, off, scale, SpriteEffects.None, 1);
            sb.DrawString(font, text, pos, col, 0f, off, scale, SpriteEffects.None, 1);
        }

    }
}
