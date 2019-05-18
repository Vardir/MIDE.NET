﻿using System.Collections.Generic;

namespace MIDE.API.Visuals
{
    public class GlyphPool
    {
        private static GlyphPool instance;
        public static GlyphPool Instance => instance ?? (instance = new GlyphPool());

        private Dictionary<string, Glyph> pool;

        public Glyph this[string key]
        {
            get
            {
                if (pool.TryGetValue(key, out Glyph g))
                    return g;
                return new Glyph('x');
            }
        }

        private GlyphPool ()
        {
            pool = new Dictionary<string, Glyph>();
        }

        public void AddOrUpdate(string key, Glyph glyph)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            if (glyph == null)
                return;
            if (pool.ContainsKey(key))
                pool[key] = glyph;
            else
                pool.Add(key, glyph);
        }
        public void Remove(string key)
        {
            pool.Remove(key);
        }

        public bool Contains(string key) => pool.ContainsKey(key);
        public Glyph Find(string key)
        {
            if (pool.TryGetValue(key, out Glyph glyph))
                return glyph;
            return null;
        }
        public Glyph GetOrAdd(string key, Glyph glyph)
        {
            if (pool.TryGetValue(key, out Glyph g))
                return g;
            pool.Add(key, glyph);
            return glyph;
        }
    }
}