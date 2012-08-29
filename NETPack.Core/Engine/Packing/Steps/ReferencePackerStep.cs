﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NETPack.Core.Engine.Structs__Enums___Interfaces;
using NETPack.Core.Engine.Utils;
using NETPack.Core.Engine.Utils.Extensions;

namespace NETPack.Core.Engine.Packing.Steps
{
    public class ReferencePackerStep : PackingStep
    {
        public ReferencePackerStep(AssemblyDefinition asmDef)
            : base(asmDef)
        {
        }

        public override string StepDescription
        {
            get { return "Packs referenced assemblies into output file..."; }
        }

        public override string StepOutput
        {
            get { return "Packing references..."; }
        }

        public override void ProcessStep()
        {
            var target = Globals.Context.TargetAssembly;
            (Globals.Context as StandardContext).MoveReferences = false;

            foreach(var markedRef in (Globals.Context as StandardContext).MarkedReferences)
            {
                var buff = QuickLZ.compress(File.ReadAllBytes(markedRef.Value), (Globals.Context as StandardContext).CompressionLevel);
                var asm = markedRef.Key;

                target.MainModule.Resources.Add(new EmbeddedResource(asm.Name.Name.MangleName(),
                                                                     ManifestResourceAttributes.Private, buff));
                Logger.VLog(string.Format("[Packing(Ref)] -> Packed reference ({0})", asm.Name.Name));
            }
        }
    }
}
