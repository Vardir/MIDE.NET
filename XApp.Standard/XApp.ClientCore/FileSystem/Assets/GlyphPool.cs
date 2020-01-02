using System.Collections.Generic;
using System.Diagnostics;

using Vardirsoft.XApp.Visuals;

namespace Vardirsoft.XApp.FileSystem
{
    public sealed class GlyphPool
    {
        private readonly Dictionary<string, Glyph> _pool;

        public Glyph this[string key] { [DebuggerStepThrough] get => _pool.TryGetValue(key, out Glyph g) ? g : new Glyph('x'); }

        public GlyphPool()
        {
            _pool = new Dictionary<string, Glyph>();
        }

        public void AddOrUpdate(string key, Glyph glyph)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            if (glyph is null)
                return;

            if (_pool.ContainsKey(key))
            {    
                _pool[key] = glyph;
            }
            else
            {    
                _pool.Add(key, glyph);
            }
        }
        
        [DebuggerStepThrough]
        public void Remove(string key) => _pool.Remove(key);

        [DebuggerStepThrough]
        public bool Contains(string key) => _pool.ContainsKey(key);
        
        [DebuggerStepThrough]
        public Glyph Find(string key) => _pool.TryGetValue(key, out Glyph glyph) ? glyph : null;

        public Glyph GetOrAdd(string key, Glyph glyph)
        {
            if (_pool.TryGetValue(key, out Glyph g))
                return g;

            _pool.Add(key, glyph);
            
            return glyph;
        }
    }
}