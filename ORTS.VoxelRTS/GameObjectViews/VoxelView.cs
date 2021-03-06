﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ORTS.Core.OpenTKHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

namespace ORTS.VoxelRTS.GameObjectViews
{
    public class VoxelView
    {
        
        float size = 0.5f;

        //X,Y,Z
        private float[] square_vertices;
        private static int number = 500000;

        //R,G,B,A
        private float[] instance_colours = new float[number*4];
        //X,Y,Z
        private float[] instance_positions = new float[number*3];
        //W,X,Y,Z
        private float[] instance_rotations = new float[number * 4];
        private ShaderProgram shader;
        int square_vao, square_vbo;
        public VoxelView()
        {
            square_vertices = new float[]{
                //Front Face
                -size, -size, size,
                size, -size, size,
                size, size, size,
                -size, size, size,
                //Back Face
                -size, -size, -size,
                -size, size, -size,
                size, size, -size,
                size, -size, -size,
                //Top Face
                -size, size, -size,
                -size, size, size,
                size, size, size,
                size, size, -size,
                //Bottom Face
                -size, -size, -size,
                size, -size, -size,
                size, -size, size,
                -size, -size, size,
                //Right Face
                size, -size, -size,
                size, size, -size,
                size, size, size,
                size, -size, size,
                //Left Face
                -size, -size, -size,
                -size, -size, size,
                -size, size, size,
                -size, size, -size
            };
            
            Random rnd = new Random();
            for (int i = 0; i < number; i++)
            {
                instance_positions[(i * 3)] = (float)(rnd.Next(-10, 10));
                instance_positions[(i * 3) + 1] = (float)(rnd.Next(-10, 10));
                instance_positions[(i * 3) + 2] = (float)(rnd.Next(-10, 10));
                instance_colours[(i * 4)] = (float)rnd.NextDouble();
                instance_colours[(i * 4) + 1] = (float)rnd.NextDouble();
                instance_colours[(i * 4) + 2] = (float)rnd.NextDouble();
                instance_colours[(i * 4) + 3] = 0.4f;
                instance_rotations[(i * 4)] = (float)rnd.NextDouble();
                instance_rotations[(i * 4) + 1] = (float)rnd.NextDouble();
                instance_rotations[(i * 4) + 2] = (float)rnd.NextDouble();
                instance_rotations[(i * 4) + 3] = (float)rnd.NextDouble();
            }

            shader = new ShaderProgram();
            shader.OnMessage += new ShaderMessageEventHandler(shader_OnMessage);
            using (StreamReader sr = new StreamReader("Shaders/instancing.vertexshader"))
            {
                shader.AddShader(ShaderType.VertexShader, sr.ReadToEnd());
            }
            using (StreamReader sr = new StreamReader("Shaders/instancing.fragmentshader"))
            {
                shader.AddShader(ShaderType.FragmentShader, sr.ReadToEnd());
            }

            GL.BindAttribLocation(shader.Program, 0, "position");
            GL.BindAttribLocation(shader.Program, 1, "instance_color");
            GL.BindAttribLocation(shader.Program, 2, "instance_position");
            GL.BindAttribLocation(shader.Program, 3, "instance_rotation");

            shader.Link();
            int square_vertices_size = square_vertices.Length * 4; 
            int instance_colours_size = instance_colours.Length * 4;
            int instance_positions_size = instance_positions.Length * 4;
            int instance_rotations_size = instance_rotations.Length * 4;
            int offset = 0;
            
            GL.GenVertexArrays(1, out square_vao);
            GL.GenBuffers(1, out square_vbo);
            GL.BindVertexArray(square_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, square_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(square_vertices_size + instance_colours_size + instance_positions_size + instance_rotations_size), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BufferSubData<float>(BufferTarget.ArrayBuffer, (IntPtr)offset, (IntPtr)square_vertices_size, square_vertices);
            offset += square_vertices_size;
            GL.BufferSubData<float>(BufferTarget.ArrayBuffer, (IntPtr)offset, (IntPtr)instance_colours_size, instance_colours);
            offset += instance_colours_size;
            GL.BufferSubData<float>(BufferTarget.ArrayBuffer, (IntPtr)offset, (IntPtr)instance_positions_size, instance_positions);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, square_vertices_size);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, square_vertices_size + instance_colours_size);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 0, square_vertices_size + instance_colours_size + instance_rotations_size);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.Arb.VertexAttribDivisor(1, 1);
            GL.Arb.VertexAttribDivisor(2, 1);
            GL.Arb.VertexAttribDivisor(3, 1);
        }

        void shader_OnMessage(string error)
        {
            Console.WriteLine(error);
        }

        public void Render(){
            shader.Enable();
            GL.BindVertexArray(square_vao);
            GL.DrawArraysInstanced(BeginMode.Quads, 0, 4*6, number);
            shader.Disable();
        }
    }
}
