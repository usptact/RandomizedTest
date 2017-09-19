using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer.Distributions;

namespace RandomizedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int numData = 10000;
            double trueP = 0.7;

            Console.WriteLine("*************");
            Console.WriteLine("True p = {0}", trueP);
            Console.WriteLine("*************\n");

            // get data
            bool[] data = GetSampleData(numData, trueP);

            Range n = new Range(numData).Named("item");

            // model variables
            Variable<double> theta = Variable.Beta(1, 1).Named("theta");        // unknown proportion of cheaters
            VariableArray<bool> x = Variable.Array<bool>(n).Named("x_data");    // array of variables for data

            // generative model
            using (Variable.ForEach(n))
            {
                var truth = Variable.Bernoulli(theta).Named("truth");
                var coinA = Variable.Bernoulli(0.5);
                var coinB = Variable.Bernoulli(0.5);
                using (Variable.If(coinA))
                    x[n] = truth;
                using (Variable.IfNot(coinA))
                {
                    using (Variable.If(coinB))
                        x[n] = true;
                    using (Variable.IfNot(coinB))
                        x[n] = false;
                }
            }

            // hook up the data
            x.ObservedValue = data;

            // inference engine uses EP by default
            InferenceEngine engine = new InferenceEngine();

            // get the mean of the posterior for theta RV
            Beta thetaMarginal = engine.Infer<Beta>(theta);

            Console.WriteLine("\n\nPosterior of theta:");
            Console.WriteLine("E(theta) = {0}", thetaMarginal.GetMean());
            Console.WriteLine("Var(theta) = {0}", thetaMarginal.GetVariance());

            Console.ReadKey();
        }

        // generate synthetic data
        // "1" - cheating
        // "0" - not cheating
        static bool[] GetSampleData(int n, double p)
        {
            bool[] data = new bool[n];

            Bernoulli z = new Bernoulli(p);
            Bernoulli fairCoin = new Bernoulli(0.5);    // fair coin; used as #1 and #2 coin as they are indistinguishable

            for (int i = 0; i < n; i++)
            {
                bool trueState = z.Sample();    // true state
                if (fairCoin.Sample())
                {
                    data[i] = trueState;        // if #1 coin = "1", report true state
                } else {
                    if (fairCoin.Sample())      // if #1 coin = "0"; toss #2 coin
                        data[i] = true;         //      report "1" if #2 coin = "1"
                    else
                        data[i] = false;        //      report "0" if #2 coin = "0"
                }
            }

            return data;
        }
    }
}
