﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing;
namespace ORTS.Test
{
    public class ARB_instanced_arraysTest : GameWindow
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct VertexPositionColor
        {
            public Vector3 Position;
            public uint Color;

            public VertexPositionColor(float x, float y, float z, Color color)
            {
                Position = new Vector3(x, y, z);
                Color = ToRgba(color);
            }

            static uint ToRgba(Color color)
            {
                return (uint)color.A << 24 | (uint)color.B << 16 | (uint)color.G << 8 | (uint)color.R;
            }
        }
        VertexPositionColor[] CubeVertices = new VertexPositionColor[]
        {
                new VertexPositionColor(-1.0f, -1.0f,  1.0f, Color.DarkRed),
                new VertexPositionColor( 1.0f, -1.0f,  1.0f, Color.DarkRed),
                new VertexPositionColor( 1.0f,  1.0f,  1.0f, Color.Gold),
                new VertexPositionColor(-1.0f,  1.0f,  1.0f, Color.Gold),
                new VertexPositionColor(-1.0f, -1.0f, -1.0f, Color.DarkRed),
                new VertexPositionColor( 1.0f, -1.0f, -1.0f, Color.DarkRed), 
                new VertexPositionColor( 1.0f,  1.0f, -1.0f, Color.Gold),
                new VertexPositionColor(-1.0f,  1.0f, -1.0f, Color.Gold) 
        };

        readonly short[] CubeElements = new short[]
        {
            0, 1, 2, 2, 3, 0, // front face
            3, 2, 6, 6, 7, 3, // top face
            7, 6, 5, 5, 4, 7, // back face
            4, 0, 3, 3, 7, 4, // left face
            0, 1, 5, 5, 4, 0, // bottom face
            1, 5, 6, 6, 2, 1, // right face
        };

        struct Vbo { public int VboID, EboID, NumElements; }
        Vbo handle;

        public ARB_instanced_arraysTest()
            : base(800, 600, new GraphicsMode(32, 24, 0, 2), "ORTS.Test")
        {
            VSync = VSyncMode.Off;

        }
        int shaderProgram = 0;


        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(new Color4(0.137f, 0.121f, 0.125f, 0f));
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int size;
            GL.GenBuffers(1, out handle.VboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(CubeVertices.Length * BlittableValueType.StrideOf(CubeVertices)), CubeVertices,
                          BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (CubeVertices.Length * BlittableValueType.StrideOf(CubeVertices) != size)
                throw new ApplicationException("Vertex data not uploaded correctly");

            GL.GenBuffers(1, out handle.EboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(CubeElements.Length * sizeof(short)), CubeElements,
                          BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (CubeElements.Length * sizeof(short) != size)
                throw new ApplicationException("Element data not uploaded correctly");

            handle.NumElements = CubeElements.Length;


            shaderProgram = GL.CreateProgram();
            int vert = GL.CreateShader(ShaderType.VertexShader);

            String vertSource = @"attribute mat4 transformmatrix;

void main()
{
    gl_Position =  gl_ModelViewProjectionMatrix * transformmatrix * gl_Vertex;
}
			";

            //
            GL.ShaderSource(vert, vertSource);
            GL.CompileShader(vert);

            string info;
            GL.GetShaderInfoLog(vert, out info);
            Console.WriteLine(info);

            int compileResult;
            GL.GetShader(vert, ShaderParameter.CompileStatus, out compileResult);
            if (compileResult != 1)
            {
                Console.WriteLine("Compile Error!");
                Console.WriteLine(vertSource);
            }

            GL.AttachShader(shaderProgram, vert);

            GL.LinkProgram(shaderProgram);

            // output link info log.
            GL.GetProgramInfoLog(shaderProgram, out info);
            Console.WriteLine(info);
            //GL.UseProgram(shaderProgram);



            if (vert != 0)
                GL.DeleteShader(vert);

            int mattest;
            Matrix4[] Cubemats = new Matrix4[]{
                Matrix4.Identity,
                Matrix4.Translation(0,10,6)
            };

            GL.GenBuffers(1, out mattest);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Cubemats.Length * BlittableValueType.StrideOf(Cubemats)), Cubemats,
              BufferUsageHint.StaticDraw);

            int pos = GL.GetAttribLocation(shaderProgram, "transformmatrix");
            int pos1 = pos + 0;
            int pos2 = pos + 1;
            int pos3 = pos + 2;
            int pos4 = pos + 3;
            GL.EnableVertexAttribArray(pos1);
            GL.EnableVertexAttribArray(pos2);
            GL.EnableVertexAttribArray(pos3);
            GL.EnableVertexAttribArray(pos4);

            GL.BindBuffer(BufferTarget.ArrayBuffer, mattest);
            GL.VertexAttribPointer(pos1, 4, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.VertexAttribPointer(pos2, 4, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, Vector3.SizeInBytes);
            GL.VertexAttribPointer(pos3, 4, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, Vector3.SizeInBytes * 2);
            GL.VertexAttribPointer(pos4, 4, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, Vector3.SizeInBytes * 4);
            GL.Arb.VertexAttribDivisor(pos1, 1);
            GL.Arb.VertexAttribDivisor(pos2, 1);
            GL.Arb.VertexAttribDivisor(pos3, 1);
            GL.Arb.VertexAttribDivisor(pos4, 1);
        }




        protected override void OnRenderFrame(FrameEventArgs e)
        {
           
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.LoadIdentity();

            Matrix4 lookat = Matrix4.LookAt(0, 0, 50, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            GL.Begin(BeginMode.Lines);
            GL.Color4(Color4.Red);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(1f, 0f, 0f);
            GL.Color4(Color4.Green);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, 1f, 0f);
            GL.Color4(Color4.Blue);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, 0f, 1f);
            GL.End();
            GL.Color4(Color4.White);

            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);
            GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(CubeVertices), new IntPtr(0));
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, BlittableValueType.StrideOf(CubeVertices), new IntPtr(12));



            //GL.UniformMatrix4(
            GL.DrawElementsInstanced(BeginMode.Triangles, handle.NumElements, DrawElementsType.UnsignedShort, IntPtr.Zero, 1);
            //GL.Arb.VertexAttribDivisor(0, 0);

            this.Title = "FPS: " + string.Format("{0:F}", 1.0 / e.Time);
            this.SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);


            if (Keyboard[Key.W])
            {
               
            }

            if (Keyboard[Key.S])
            {
        
            }

            if (Keyboard[Key.A])
            {

            }

            if (Keyboard[Key.D])
            {

            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Width / (float)Height, 1, 128);
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
    }
}