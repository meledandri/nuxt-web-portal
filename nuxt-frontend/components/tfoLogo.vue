<template>
  <img
    class="AppLogo"
    alt="TechFile Online Logo"
    src="/tfo-logo.png"
  >
</template>

<style>
.AppLogo {
  height: 30px;
  width: 180px;
  transform: rotateY(560deg);
  animation: turn 3.5s ease-out forwards 1s;
}

@keyframes turn {
  100% {
    transform: rotateY(0deg);
  }
}
</style>
