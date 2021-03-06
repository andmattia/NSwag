﻿using System.Threading.Tasks;
using Xunit;
using NSwag.SwaggerGeneration.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class AngularTests
    {
        public class Foo
        {
            public string Bar { get; set; }
        }

        [Route("[controller]/[action]")]
        public class DiscussionController : Controller
        {
            [HttpPost]
            public void AddMessage([FromBody]Foo message)
            {
            }

            [HttpPost]
            public void GenericRequestTest1(GenericRequest1 request)
            {

            }

            [HttpPost]
            public void GenericRequestTest2(GenericRequest2 request)
            {

            }
        }

        public class GenericRequestBase<T>
            where T : RequestBodyBase
        {
            [Required]
            public T Request { get; set; }
        }

        public class RequestBodyBase
        {

        }

        public class RequestBody : RequestBodyBase
        {

        }

        public class GenericRequest1 : GenericRequestBase<RequestBodyBase>
        {

        }

        public class GenericRequest2 : GenericRequestBase<RequestBody>
        {

        }

        [Fact]
        public async Task When_return_value_is_void_then_client_returns_observable_of_void()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("addMessage(message: Foo | null): Observable<void>", code);
        }

        [Fact]
        public async Task When_export_types_is_true_then_add_export_before_classes()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    ExportTypes = true
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("export class DiscussionClient", code);
            Assert.Contains("export interface IDiscussionClient", code);
        }

        [Fact]
        public async Task When_export_types_is_false_then_dont_add_export_before_classes()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    ExportTypes = false
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.DoesNotContain("export class DiscussionClient", code);
            Assert.DoesNotContain("export interface IDiscussionClient", code);
        }

        [Fact]
        public async Task When_generic_request()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateDtoTypes = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.7m,
                    ExportTypes = false
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("this.request = new RequestBodyBase()", code);
            Assert.Contains("this.request = new RequestBody()",     code);
        }
    }
}
