﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsmResolver.Net.Metadata;

namespace AsmResolver.Net.Signatures
{
    public class GenericParameterSignature : TypeSignature
    {
        public static GenericParameterSignature FromReader(MetadataHeader header, IBinaryStreamReader reader, GenericParameterType parameterType)
        {
            long position = reader.Position;

            uint index;
            if (!reader.TryReadCompressedUInt32(out index))
                return null;

            return new GenericParameterSignature(parameterType, (int) index)
            {
                StartOffset = position
            };
        }

        public GenericParameterSignature(GenericParameterType parameterType, int index)
        {
            ParameterType = parameterType;
            Index = index;
        }

        public override ElementType ElementType
        {
            get
            {
                switch (ParameterType)
                {
                    case GenericParameterType.Type:
                        return ElementType.Var;
                    case GenericParameterType.Method:
                        return ElementType.MVar;
                }
                throw new NotSupportedException();
            }
        }
        
        public GenericParameterType ParameterType
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public override string Name
        {
            get
            {
                switch (ParameterType)
                {
                    case GenericParameterType.Method:
                        return "!!" + Index.ToString();
                    case GenericParameterType.Type:
                        return "!" + Index.ToString();
                }
                throw new NotSupportedException();
            }
        }

        public override string Namespace
        {
            get { return string.Empty; }
        }

        public override IResolutionScope ResolutionScope
        {
            get { return null; }
        }

        public override uint GetPhysicalLength()
        {
            return sizeof (byte) +
                   Index.GetCompressedSize();
        }

        public override void Write(WritingContext context)
        {
            var writer = context.Writer;
            writer.WriteByte((byte)ElementType);
            writer.WriteCompressedUInt32((uint)Index);
        }
    }

    public enum GenericParameterType
    {
        Type,
        Method,
    }
}
