﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ORTS.Core.Graphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ORTS.Core.Maths;

namespace ORTS.Core.OpenTKHelper
{
    public class Camera: ICamera
    {
        public Quat rotation { get; private set; }
        public Vect3 postion { get; private set; }
        public Camera()
        {
            rotation = Quat.Identity;
            postion = Vect3.Zero;
        }
        public void Rotate(Quat q)
        {
            rotation *= q;
        }
        public void Translate(Vect3 v)
        {
            postion += -v;
        }
        public Matrix4 toMatrix4()
        {
            return new Matrix4();
        }
    }
}
