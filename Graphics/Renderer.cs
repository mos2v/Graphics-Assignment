/* مصطفى سعيد عبدالفضيل عبدالسميع
 * 2021170522
 * sec 6
 * Cs
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

//include GLM library
using GlmNet;


using System.IO;
using System.Diagnostics;
using System.Security.Principal;

namespace Graphics
{
    class Renderer
    {
        Shader sh;
        
        uint circleBufferID;
        uint xyzAxesBufferID;
        uint coneBufferID;
        uint cubeBufferID;
        //uint hemisphereBufferID;

        //3D Drawing
        mat4 ModelMatrix;
        mat4 ModelMatrix2;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;
        
        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        const int CircleEdges = 50;
        const float rotationSpeed = 1f;
        float rotationAngle = 0;

        public float translationX=0, 
                     translationY=0, 
                     translationZ=0;

        vec3 scale = new vec3(1);

        Stopwatch timer = Stopwatch.StartNew();

        Texture cubeTexture;

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0.4f, 1);


            cubeTexture = new Texture(projectPath + "\\Textures\\cube.jpg", 1);



            float[] cube =
            {
                //bottom
                0.0f, 30.0f, 0.0f, 0.0f, 0.0f, 0.0f,  0, 0,
                0.0f, 50.0f, 0.0f, 0.0f, 0.0f, 0.0f,  1, 0,
                20.0f, 50.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1, 1,
                20.0f, 30.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0, 1,

                

                //
                0.0f, 30.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0, 0,
                0.0f, 30.0f, 20.0f, 0.0f, 0.0f, 0.0f, 1, 0,
                0.0f, 50.0f, 20.0f, 0.0f, 0.0f, 0.0f, 1, 1,
                0.0f, 50.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0, 1,

                //
                20.0f, 50.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0, 0,
                0.0f, 50.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1, 0,
                0.0f, 50.0f, 20.0f, 0.0f, 0.0f, 0.0f, 1, 1,
                20.0f, 50.0f, 20.0f, 0.0f, 0.0f, 0.0f, 0, 1,

                //
                0.0f, 30.0f, 20.0f, 0.0f, 0.0f, 0.0f, 0, 0,
                0.0f, 30.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1, 0,
                20.0f, 30.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1, 1,
                20.0f, 30.0f, 20.0f, 0.0f, 0.0f, 0.0f, 0, 1,
                
                //
                20.0f, 30.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0, 0,
                20.0f, 50.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1, 0,
                20.0f, 50.0f, 20.0f, 0.0f, 0.0f, 0.0f, 1, 1,
                20.0f, 30.0f, 20.0f, 0.0f, 0.0f, 0.0f, 0, 1,

                //top
                0.0f, 30.0f, 20.0f, 0.0f, 0.0f, 0.0f, 0, 0,
                0.0f, 50.0f, 20.0f, 0.0f, 0.0f, 0.0f, 1, 0,
                20.0f, 50.0f, 20.0f, 0.0f, 0.0f, 0.0f, 1, 1,
                20.0f, 30.0f, 20.0f, 0.0f, 0.0f, 0.0f, 0, 1,

            };

            float[] xyzAxesVertices = {
		        //x
		        0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R
		        100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R
		        //y
	            0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
		        0.0f, 100.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
		        //z
	            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,  //B
		        0.0f, 0.0f, 100.0f, 0.0f, 0.0f, 1.0f,  //B
            };

            float[] circleVertices = draw_circle(5, 5, 10, 10, 0, 1, 0);
            float[] coneVertices = draw_Cone(5, 5, 10, 10, 50, 1, 1, 0);
            //float[] hemisphere = draw_hemisphere(40, 20, 7, 0, 1, 1);

            circleBufferID = GPU.GenerateBuffer(circleVertices);
            xyzAxesBufferID = GPU.GenerateBuffer(xyzAxesVertices);
            coneBufferID = GPU.GenerateBuffer(coneVertices);
            cubeBufferID = GPU.GenerateBuffer(cube);
            //hemisphereBufferID = GPU.GenerateBuffer(hemisphere);

            // View matrix 
            ViewMatrix = glm.lookAt(
                        new vec3(50, 50, 50), // Camera is at (0,5,5), in World Space
                        new vec3(0, 0, 0), // and looks at the origin
                        new vec3(0, 0, 1)  // Head is up (set to 0,-1,0 to look upside-down)
                );
            // Model Matrix Initialization
            ModelMatrix = new mat4(1);
            ModelMatrix2 = new mat4(1);
            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);
            
            // Our MVP matrix which is a multiplication of our 3 matrices 
            sh.UseShader();


            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");

            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            timer.Start();
        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            #region XYZ axis
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, xyzAxesBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, new mat4(1).to_array()); // Identity

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
             
            Gl.glDrawArrays(Gl.GL_LINES, 0, 6);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            #endregion


            #region Cone
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, coneBufferID);
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix2.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            
            Gl.glDrawArrays(Gl.GL_TRIANGLE_FAN, 0, (CircleEdges) + 2);
            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
           
            #endregion

            #region Circle
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, circleBufferID);
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix2.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 0, CircleEdges);
            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region cube
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cubeBufferID);
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));
            cubeTexture.Bind();
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 6 * 4);
            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
            #endregion

            //#region hemisphere
            //Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hemisphereBufferID);
            //Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, new mat4(1).to_array());
            //Gl.glEnableVertexAttribArray(0);
            //Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            //Gl.glEnableVertexAttribArray(1);
            //Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            //Gl.glDrawArrays(Gl.GL_QUAD_STRIP, 0, 66);
            //Gl.glDisableVertexAttribArray(0);
            //Gl.glDisableVertexAttribArray(1);
            //#endregion
        }
        public float[] draw_circle(float centerX, float centerY, float centerZ, float radius, float R, float G, float B)
        {
            List<float> verticies = new List<float>();

            float step = (float)(2 * Math.PI) / CircleEdges;

            float angle = 0.0f;
            while (angle < 2 * Math.PI)
            {
                float x = centerX + (float)(radius * Math.Cos(angle));
                float y = centerY + (float)(radius * Math.Sin(angle));
                verticies.AddRange(new float[] { x, y, centerZ, R, G, B });
                angle += step;
            }

            return verticies.ToArray();
        }
        public float[] draw_Cone(float centerX, float centerY, float centerZ, float radius, float height, float R, float G, float B)
        {
           
            List<float> verticies = new List<float>();
            verticies.AddRange(new float[] { 0, 0, -height, R, G, B });
            for (int i = 0; i <= CircleEdges; i++)
            {
                float angle = (float)i / CircleEdges * 2.0f * (float)Math.PI;
                
                float x = centerX + (float)Math.Cos(angle) * radius;
                float y = centerY + (float)Math.Sin(angle) * radius;
                verticies.AddRange(new float[] { x, y, centerZ, R, G, B});
            }
            
            return verticies.ToArray();
        }
        //public float[] draw_hemisphere(float latitudeLines, float longitudeLines, float radius, float R, float G, float B)
        //{
        //    List<float> verticies = new List<float>();

        //    for (int lat = 0; lat <= latitudeLines / 2; lat++) // Change here to draw only half
        //    {
        //        float a1 = (float)Math.PI * (lat - 1) / latitudeLines;
        //        float sinA1 = (float)Math.Sin(a1);
        //        float cosA1 = (float)Math.Cos(a1);

        //        float a2 = (float)Math.PI * lat / latitudeLines;
        //        float sinA2 = (float)Math.Sin(a2);
        //        float cosA2 = (float)Math.Cos(a2);

        //        for (int lon = 0; lon <= longitudeLines; lon++)
        //        {
        //            float b = 2 * (float)Math.PI * lon / longitudeLines;
        //            float sinB = (float)Math.Sin(b);
        //            float cosB = (float)Math.Cos(b);

        //            float x1 = cosB * sinA1;
        //            float y1 = sinB * sinA1;
        //            float z1 = cosA1;

        //            float x2 = cosB * sinA2;
        //            float y2 = sinB * sinA2;
        //            float z2 = cosA2;

                    
        //            verticies.AddRange(new float[] { radius * x2, radius * y2, radius * z2 });
        //            verticies.AddRange(new float[] { radius * x1, radius * y1, radius * z1 });
        //        }
        //    }
        //    return verticies.ToArray();
        //}
        public void Update()
        {
            vec3 coneCenter = new vec3(5f, 5f, 30);
            vec3 cubeCenter = new vec3(10, 25, 10);
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds / 1000.0f;
            rotationAngle += deltaTime * rotationSpeed;

            List<mat4> cubetransformations = new List<mat4>();
            cubetransformations.Add(glm.scale(new mat4(1), scale));
            cubetransformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));

            ModelMatrix = MathHelper.MultiplyMatrices(cubetransformations);

            List<mat4> coneTransformations = new List<mat4>();
            coneTransformations.Add(glm.translate(new mat4(1), -1 * coneCenter));
            coneTransformations.Add(glm.rotate(rotationAngle, new vec3(0, 0, 1)));
            coneTransformations.Add(glm.translate(new mat4(1), coneCenter));

            ModelMatrix2 = MathHelper.MultiplyMatrices(coneTransformations);

            timer.Reset();
            timer.Start();
        }
        public void OnKeyboardpress(char k)
        {
            float speed = 5;

            if (k == 'd')
                translationX += speed;
            if (k == 'a')
                translationX -= speed;

            if (k == 'w')
                translationY += speed;
            if (k == 's')
                translationY -= speed;

            if (k == 'z')
                translationZ += speed;
            if (k == 'c')
                translationZ -= speed;

            if (k == 'u')
            {
                scale.x *= 1.2f;
                scale.y *= 1.2f;
                scale.z *= 1.2f;
            }

            if (k == 'j')
            {
                scale.x /= 1.2f;
                scale.y /= 1.2f;
                scale.z /= 1.2f;
            }
        }

        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
