using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace McAfeeLabs.Engineering.Automation.Base.Json
{
    public class JsonRootCompiler
    {
        private SerializationModel serializationModel;
        private string language;

        public JsonRootCompiler(string language, SerializationModel serializationModel)
        {
            this.language = language;
            this.serializationModel = serializationModel;
        }

        public void GenerateCode(JsonRoot root, TextWriter writer)
        {
            CodeCompileUnit result = new CodeCompileUnit();
            result.Namespaces.Add(new CodeNamespace());
            GenerateType(result.Namespaces[0], root, new List<string>());
            CodeDomProvider provider = CodeDomProvider.CreateProvider(this.language);
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            provider.GenerateCodeFromCompileUnit(result, writer, options);
        }

        private string GenerateType(CodeNamespace ns, JsonRoot root, List<string> existingTypes)
        {
            if (!root.IsUserDefinedType) return null;

            CodeTypeDeclaration rootType = new CodeTypeDeclaration(GetUniqueDataContractName(root.UserDefinedTypeName, existingTypes));
            existingTypes.Add(rootType.Name);
            rootType.Attributes = MemberAttributes.Public;
            rootType.IsPartial = true;
            rootType.IsClass = true;
            ns.Types.Add(rootType);
            rootType.Comments.Add(
                new CodeCommentStatement(
                    string.Format(
                        "Type created for JSON at {0}",
                        string.Join(" --> ", root.GetAncestors()))));

            AddAttributeDeclaration(rootType, rootType.Name, root.UserDefinedTypeName);
            AddMembers(ns, rootType, root, existingTypes);
            return rootType.Name;
        }

        private void AddMembers(CodeNamespace ns, CodeTypeDeclaration typeDecl, JsonRoot jsonRoot, List<string> existingTypes)
        {
            foreach (var memberName in jsonRoot.Members.Keys)
            {
                string fieldName = EscapeFieldName(memberName);
                var member = jsonRoot.Members[memberName];
                CodeTypeReference fieldType;
                if (member.IsUserDefinedType)
                {
                    string fieldTypeName = this.GenerateType(ns, member, existingTypes);
                    fieldType = new CodeTypeReference(fieldTypeName);
                }
                else
                {
                    fieldType = new CodeTypeReference(member.ElementType ?? typeof(object));
                }

                for (int i = 0; i < member.ArrayRank; i++)
                {
                    fieldType = new CodeTypeReference(fieldType, 1);
                }

                // TODO: implement members as properties instead of fields
                CodeMemberField field = new CodeMemberField(fieldType, fieldName);
                field.Attributes = MemberAttributes.Public;
                typeDecl.Members.Add(field);

                this.AddMemberAttributeDeclaration(field, memberName);
            }
        }

        private void AddMemberAttributeDeclaration(CodeMemberField field, string jsonName)
        {
            if (this.serializationModel == SerializationModel.DataContractJsonSerializer)
            {
                CodeAttributeDeclaration attr = new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(DataMemberAttribute)));
                if (field.Name != jsonName)
                {
                    attr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(jsonName)));
                }

                field.CustomAttributes.Add(attr);
            }
            else if (this.serializationModel == SerializationModel.JsonNet)
            {
                CodeAttributeDeclaration attr = new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(JsonPropertyAttribute)));
                if (field.Name != jsonName)
                {
                    attr.Arguments.Add(new CodeAttributeArgument("PropertyName", new CodePrimitiveExpression(jsonName)));
                }

                field.CustomAttributes.Add(attr);
            }
            else
            {
                throw new ArgumentException("Invalid serialization model");
            }
        }

        private void AddAttributeDeclaration(CodeTypeDeclaration typeDecl, string clrTypeName, string jsonName)
        {
            if (this.serializationModel == SerializationModel.DataContractJsonSerializer)
            {
                CodeAttributeDeclaration attr = new CodeAttributeDeclaration(
                        new CodeTypeReference(typeof(DataContractAttribute)));
                if (clrTypeName != jsonName)
                {
                    attr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(jsonName)));
                }

                typeDecl.CustomAttributes.Add(attr);
            }
            else if (this.serializationModel == SerializationModel.JsonNet)
            {
                typeDecl.CustomAttributes.Add(
                    new CodeAttributeDeclaration(
                        new CodeTypeReference(typeof(JsonObjectAttribute)),
                        new CodeAttributeArgument(
                            "MemberSerialization",
                            new CodeFieldReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(MemberSerialization)),
                                "OptIn"))));
            }
            else
            {
                throw new ArgumentException("Invalid serialization model");
            }
        }

        private string EscapeFieldName(string jsonName)
        {
            const string LettersAndUnderline = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
            const string Numbers = "0123456789";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < jsonName.Length; i++)
            {
                bool ok = LettersAndUnderline.Contains(jsonName[i]) || (i > 0 && Numbers.Contains(jsonName[i]));
                if (ok)
                {
                    sb.Append(jsonName[i]);
                }
                else
                {
                    sb.AppendFormat("_{0:X4}_", (int)jsonName[i]);
                }
            }

            // special case, empty key
            if (sb.Length == 0)
            {
                sb.Append("__empty__");
            }

            return sb.ToString();
        }

        private string GetUniqueDataContractName(string name, IEnumerable<string> existingTypes)
        {
            string baseName = EscapeFieldName(name);
            baseName = Char.ToUpperInvariant(baseName[0]) + baseName.Substring(1);

            string result = baseName;
            int index = 1;
            while (existingTypes.Contains(result))
            {
                result = baseName + index;
                index++;
            }

            return result;
        }
    }
}
