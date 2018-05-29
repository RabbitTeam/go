﻿namespace Rabbit.Go.Features
{
    public struct FeatureReference<T>
    {
        private T _feature;
        private int _revision;

        private FeatureReference(T feature, int revision)
        {
            _feature = feature;
            _revision = revision;
        }

        public static readonly FeatureReference<T> Default = new FeatureReference<T>(default(T), -1);

        public T Fetch(IFeatureCollection features)
        {
            if (_revision == features.Revision)
            {
                return _feature;
            }
            _feature = (T)features[typeof(T)];
            _revision = features.Revision;
            return _feature;
        }

        public T Update(IFeatureCollection features, T feature)
        {
            features[typeof(T)] = feature;
            _feature = feature;
            _revision = features.Revision;
            return feature;
        }
    }
}