<script setup lang="ts">
const props = defineProps<{
  value: string[]
}>()

defineOptions({
  name: 'AuthAll',
})

function check() {
  return useAuth().authAll(props.value)
}
</script>

<template>
  <div>
    <slot v-if="check()" />
    <slot v-else name="no-auth" />
  </div>
</template>
