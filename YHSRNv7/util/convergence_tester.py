import numpy as np

class ConvergenceTester:
	def __init__(self, threshold = 0, steps = 500):
		self.__threshold = threshold
		self.__steps = steps
		self.__losses = np.empty(self.__steps)
		self.__losses[:] = np.nan

	def plunge(self, loss):
		self.__losses = self.__losses[1:]
		self.__losses = np.append(self.__losses, float(str(loss)))

	def clear(self):
		self.__losses = np.empty(self.__steps)
		self.__losses[:] = np.nan

	def __is_convergence(self):
		return self.r > self.__threshold

	def __r(self):
		if np.any(np.isnan(self.__losses)):
			return np.nan

		return np.corrcoef(np.arange(self.__steps), self.__losses)[1,0]

	is_convergence = property(__is_convergence)
	r = property(__r)