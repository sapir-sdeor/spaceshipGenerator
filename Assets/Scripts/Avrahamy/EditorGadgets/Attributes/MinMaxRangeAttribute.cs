using System;

namespace Avrahamy.EditorGadgets {
    public class MinMaxRangeAttribute : Attribute {
        public readonly float min;
        public readonly float max;

        public MinMaxRangeAttribute(float min, float max) {
            this.min = min;
            this.max = max;
        }
    }
}
